using AutoMapper;
using Business.Interfaces;
using DataAccess;
using DataAccess.Entities;
using DataTransferObject;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Repository;
using StaticClass;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnitOfWork;

namespace Business
{
    public  class LoginBL : ILoginBL
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IMapper _mapper;
        private readonly IParametrosBL _parametrosBL;
        private readonly UserManager<UserProfile> _userManager;
        public LoginBL(IUnitOfWorkFactory uowFactory, IParametrosBL parametrosBL, UserManager<UserProfile> userManager)
        {
            _uowFactory = uowFactory;
            _parametrosBL = parametrosBL;
            _userManager= userManager;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Provincias, ProvinciaDTO>();
                cfg.CreateMap<TiposDeDocumentos, TiposDeDocumentoDTO>();
                cfg.CreateMap<TiposDeDocumentos, TiposDeDocumentoDTO>();

            });
            _mapper = config.CreateMapper();
            _userManager = userManager;
        }
        public Task<string> GetUrlLogin()
        {
            var uow = _uowFactory.GetUnitOfWork();
            var repoParam = new ParametrosRepository(uow);
            string result = null;
            
            string UrlBase = repoParam.GetParametroChar("SUU.Rest.UrlBase");
            string UrlRedirect = repoParam.GetParametroChar("SUU.Rest.redirect_uri");
            string ClientId = repoParam.GetParametroChar("SUU.Rest.client_id");
            string state = Functions.GenerateRandomString(40);
            result = $"{UrlBase}/oauth/gam/signin?oauth=auth&redirect_uri={UrlRedirect}&client_id={ClientId}&scope=gam_user_data&state={state}";
          
            return Task.FromResult(result);
        }
        
        public async Task<UserInfoDto> GetUsuarioLogueado(string code, string state)
        {
            UserInfoDto user = null;
            var uow = _uowFactory.GetUnitOfWork();
            var repoLoginSuu = new LogLoginSuuRepository(uow);

            var AccessToken = await GetAccessToken(code);
            user = await GetUserInfo(AccessToken.access_token);

            var entity = await repoLoginSuu.FirstOrDefaultAsync(x => x.Username == user.usuario);

            if (entity == null)
            {
                entity = new LogLoginSuu
                {
                    State = state,
                    AccessToken = AccessToken.access_token,
                    Username = user.usuario,
                    LastUpdateDate = DateTime.Now
                };

                await repoLoginSuu.AddAsync(entity);
            }
            else
            {
                entity.State = state;
                entity.AccessToken = AccessToken.access_token;
                entity.LastUpdateDate = DateTime.Now;
                await repoLoginSuu.UpdateAsync(entity);
            }

            return user;
        }
        private async Task<UserInfoDto> GetUserInfo(string token)
        {
            var uow = _uowFactory.GetUnitOfWork();
            var repoParam = new ParametrosRepository(uow);

            string _urlSUU = repoParam.GetParametroChar("SUU.Rest.UrlBase");

            HttpClient client = new HttpClient();
            UserInfoDto user;

            try
            {
                string uriString = $"{_urlSUU}/oauth/gam/userinfo";

                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uriString);
                req.Headers.Add("Authorization", token);

                var response = await client.SendAsync(req);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK )
                {
                    user = JsonConvert.DeserializeObject<UserInfoDto>(responseContent);
                }
                else
                {
                    throw new ArgumentException("Error no contemplado: " + responseContent);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            finally
            {
                client.Dispose();
            }

            return user;
        }
        private async Task<AccessTokenResponseDto> GetAccessToken(string code)
        {
            var uow = _uowFactory.GetUnitOfWork();
            var repoParam = new ParametrosRepository(uow);


            string _urlSUU = repoParam.GetParametroChar("SUU.Rest.UrlBase");
            string client_id = repoParam.GetParametroChar("SUU.Rest.client_id");
            string client_secret = repoParam.GetParametroChar("SUU.Rest.client_secret");
            string redirect_uri = repoParam.GetParametroChar("SUU.Rest.redirect_uri");

            HttpClient client = new HttpClient();
            AccessTokenResponseDto access_token;

            try
            {
                string uriString = $"{_urlSUU}/oauth/gam/access_token";

                var body = StaticClass.Helpers.ReflectionHelpers.ToDictionary(new AccessTokenRequestDto
                {
                    code = code,
                    client_id = client_id,
                    client_secret = client_secret,
                    redirect_uri = redirect_uri,
                    grant_type = "authorization_code"
                });

                var content = new FormUrlEncodedContent(body);
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uriString);
                req.Content = content;

                var response = await client.SendAsync(req);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                {
                    access_token = JsonConvert.DeserializeObject<AccessTokenResponseDto>(responseContent);
                }
                else if (response.StatusCode == HttpStatusCode.ExpectationFailed)
                {
                    var error = JsonConvert.DeserializeObject<ErrorSuuDto>(responseContent);
                    throw new ArgumentException("Error : " + error.Message);
                }
                else
                {
                    throw new ArgumentException("Error no contemplado: " + responseContent);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            finally
            {
                client.Dispose();
            }

            return access_token;

        }
        public async Task<string> GetUrlLogout(string username,string UrlRedirect)
        {
            var uow = _uowFactory.GetUnitOfWork();
            var repoParam = new ParametrosRepository(uow);
            var repoLogLoginSuu = new LogLoginSuuRepository(uow);
            string uriString = null!;

            var entity = await repoLogLoginSuu.FirstOrDefaultAsync(x => x.Username == username);

            if(entity != null)
            {
                string _urlSUU = repoParam.GetParametroChar("SUU.Rest.UrlBase");
                string client_id = repoParam.GetParametroChar("SUU.Rest.client_id");
                string client_secret = repoParam.GetParametroChar("SUU.Rest.client_secret");
                string repository_id = repoParam.GetParametroChar("SUU.Rest.repository_id");
                uriString = $"{_urlSUU}/oauth/gam/signout?oauth=signout&client_id={client_id}&client_secret={client_secret}&server_ip=1&redirect_uri={UrlRedirect}&token={entity.AccessToken}&first_call=1&state={entity.State}&repository_id={repository_id}";
            }
            return uriString;
        }

    }
}
