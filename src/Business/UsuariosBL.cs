using Ardalis.GuardClauses;
using AutoMapper;
using Business.Interfaces;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UnitOfWork;
using Microsoft.AspNetCore.Components.Authorization;
using Repository;
using DataTransferObject;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace Business
{
    public class UsuariosBL: IUsuariosBL
    {
        
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IMapper _mapper;
        private readonly SignInManager<UserProfile> _signInManager;
        private readonly AuthenticationStateProvider _authStaterProvider;
        private readonly IConfiguration _configuration;

        public UsuariosBL(IUnitOfWorkFactory uowFactory, AuthenticationStateProvider authStaterProvider, SignInManager<UserProfile> signInManager, IConfiguration configuration)
        {
            _uowFactory = Guard.Against.Null(uowFactory);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Perfiles, PerfilDTO>();
                cfg.CreateMap<UserProfile, UsuarioDTO>().ReverseMap();
                cfg.CreateMap<Permisos, PermisoDTO>().ReverseMap();
                cfg.CreateMap<Empresas, EmpresaDTO>().ReverseMap();

            });
            _mapper = config.CreateMapper();

            _signInManager = signInManager;
            _authStaterProvider = authStaterProvider;
            _configuration = configuration;

        }

        public async Task<string> GetCurrentIdEmpresa()
        {
            //devuelve el id del usuario representado
            string result = null;
            var authState = await _authStaterProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity?.IsAuthenticated ?? false)
            {
                result = authState.User.FindFirst("IdEmpresa")?.Value ?? "";
            }
            return result;
        }
        public async Task<string> GetCurrentUserNameEmpresa()
        {
            //devuelve el Username del usuario representado
            string result = null;
            var authState = await _authStaterProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity?.IsAuthenticated ?? false)
            {
                result = authState.User.FindFirst("CuitEmpresa")?.Value ?? "";
            }
            return result;
        }
        public async Task<string> GetCurrentNombreEmpresa()
        {
            //devuelve el Username del usuario representado
            string result = null;
            var authState = await _authStaterProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity?.IsAuthenticated ?? false)
            {
                result = authState.User.FindFirst("NombreEmpresa")?.Value ?? "";
            }
            return result;
        }
        public async Task<string> GetCurrentUserName()
        {
            //devuelve el Username del usuario logueado
            string result = null;
            var authState = await _authStaterProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity?.IsAuthenticated ?? false)
            {
                result = authState.User.FindFirst(ClaimTypes.Name).Value;
            }
            return result;
        }
        public async Task<string> GetCurrentUserid()
        {
            //devuelve el id del usuario logueado
            string result = null;
            var authState = await _authStaterProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity?.IsAuthenticated ?? false)
            {
                result = authState.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            return result;
        }
        public async Task<string> GetCurrentNombreyApellido()
        {
            //devuelve el Username del usuario representado
            string result = null;
            var authState = await _authStaterProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity?.IsAuthenticated ?? false)
            {
                result = authState.User.FindFirst("NombreyApellido")?.Value ?? "";
            }
            return result;
        }

        public async Task<UserProfile> GetUserByNameAsync(string username)
        {
            return await _signInManager.UserManager.FindByNameAsync(username);
        }
        public async Task<List<UsuarioDTO>> GetUsersAsync()
        {
            var uof = _uowFactory.GetUnitOfWork();
            var repoRel =new  RelUsuariosPerfilesRepository(uof);
            
            var lstUsers = await _signInManager.UserManager.Users.ToListAsync();
            var lstUsuariosPerfiles = repoRel.DbSet.ToList();

            var usuarios = (from usu in lstUsers
                            select new UsuarioDTO
                            {
                                Id = usu.Id,
                                AccessFailedCount = usu.AccessFailedCount,
                                ConcurrencyStamp = usu.ConcurrencyStamp,
                                Email = usu.Email,
                                EmailConfirmed = usu.EmailConfirmed,
                                LockoutEnabled = usu.LockoutEnabled,
                                LockoutEnd = usu.LockoutEnd,
                                NormalizedEmail = usu.NormalizedEmail,
                                NormalizedUserName = usu.NormalizedUserName,
                                PasswordHash = usu.PasswordHash,
                                PhoneNumber = usu.PhoneNumber,
                                PhoneNumberConfirmed = usu.PhoneNumberConfirmed,
                                SecurityStamp = usu.SecurityStamp,
                                TwoFactorEnabled = usu.TwoFactorEnabled,
                                UserName = usu.UserName,
                                Estado = usu.Estado,
                                NombreyApellido = usu.NombreyApellido,
                                Perfiles = _mapper.Map<List<Perfiles>,List<PerfilDTO>>(
                                                lstUsuariosPerfiles
                                                    .Where(x => x.Userid == usu.Id)
                                                    .Select(s => s.IdPerfilNavigation)
                                                    .ToList()
                                            )
                            }).ToList();


            return usuarios;
        }
        public async Task<List<UsuarioDTO>> GetUsersAsync(string username, int? IdPerfil, int? IdEstado )
        {
            var uof = _uowFactory.GetUnitOfWork();
            var repoRel = new RelUsuariosPerfilesRepository(uof);

            var lstUsers = await _signInManager.UserManager.Users.ToListAsync();
            var lstUsuariosPerfiles = repoRel.DbSet.ToList();

            var usuarios = (from usu in lstUsers
                            select new UsuarioDTO
                            {
                                Id = usu.Id,
                                AccessFailedCount = usu.AccessFailedCount,
                                ConcurrencyStamp = usu.ConcurrencyStamp,
                                Email = usu.Email,
                                EmailConfirmed = usu.EmailConfirmed,
                                LockoutEnabled = usu.LockoutEnabled,
                                LockoutEnd = usu.LockoutEnd,
                                NormalizedEmail = usu.NormalizedEmail,
                                NormalizedUserName = usu.NormalizedUserName,
                                PasswordHash = usu.PasswordHash,
                                PhoneNumber = usu.PhoneNumber,
                                PhoneNumberConfirmed = usu.PhoneNumberConfirmed,
                                SecurityStamp = usu.SecurityStamp,
                                TwoFactorEnabled = usu.TwoFactorEnabled,
                                UserName = usu.UserName,
                                Estado = usu.Estado,
                                NombreyApellido = usu.NombreyApellido,
                                Perfiles = _mapper.Map<List<Perfiles>, List<PerfilDTO>>(
                                                lstUsuariosPerfiles
                                                    .Where(x => x.Userid == usu.Id)
                                                    .Select(s => s.IdPerfilNavigation)
                                                    .ToList()
                                            )
                            });

            if(!string.IsNullOrWhiteSpace(username))
                usuarios = usuarios.Where(x => x.UserName.Contains(username));

            if (IdPerfil.HasValue)
                usuarios = usuarios.Where(x => x.Perfiles.Any(x => x.IdPerfil == IdPerfil));

            if (IdEstado.HasValue)
                usuarios = usuarios.Where(x => x.Estado == IdEstado);



            return usuarios.ToList();
        }

        public async Task<UsuarioDTO> GetUserByIdAsync(string Id)
        {
            var uof = _uowFactory.GetUnitOfWork();
            var repoRel = new RelUsuariosPerfilesRepository(uof);

            var usu = await _signInManager.UserManager.FindByIdAsync(Id);
            var lstUsuarioPerfiles = repoRel.DbSet.Where(x=> x.Userid == Id).Select(s=> s.IdPerfilNavigation).ToList();

            var usuario = _mapper.Map<UserProfile, UsuarioDTO>(usu);
            usuario.Perfiles = _mapper.Map<List<Perfiles>, List<PerfilDTO>>(lstUsuarioPerfiles);

            return usuario;
        }
        public async Task<UsuarioDTO> AgregarUsuarioAsync(UsuarioDTO dto)
        {
            UsuarioDTO result= null;

            UserProfile usuario = _mapper.Map<UsuarioDTO,UserProfile>(dto);
            usuario.Id = Guid.NewGuid().ToString();
            IdentityResult resultCreate = await _signInManager.UserManager.CreateAsync(usuario);

            if (resultCreate.Succeeded)
            {
                result = _mapper.Map<UsuarioDTO>(await _signInManager.UserManager.FindByNameAsync(usuario.UserName));
                await ActualizarPerfilesAsync(dto.Perfiles, usuario.Id);
            }
            else
                throw new Exception(string.Join(", ", resultCreate.Errors.Select(s => s.Description).ToList()));

            return result;
        }
        public async Task<UsuarioDTO> ActualizarUsuarioAsync(UsuarioDTO dto)
        {
            UsuarioDTO result = null;

            var usuario = await _signInManager.UserManager.FindByIdAsync(dto.Id);
            _mapper.Map<UsuarioDTO, UserProfile>(dto, usuario);

            IdentityResult resultUpdate  = await _signInManager.UserManager.UpdateAsync(usuario);
            if (resultUpdate.Succeeded)
            {
                result = _mapper.Map<UsuarioDTO>(usuario);
                await ActualizarPerfilesAsync(dto.Perfiles,usuario.Id);
            }
            else
                throw new Exception(string.Join(", ", resultUpdate.Errors.Select(s => s.Description).ToList()));

            return result;
        }
        private async Task ActualizarPerfilesAsync(List<PerfilDTO> perfiles,string userid)
        {
            var currentUserid = await GetCurrentUserid();

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new RelUsuariosPerfilesRepository(uow);

                    var perfilesEntity = repo.DbSet.Where(x => x.Userid == userid ).ToList();
                    await repo.RemoveRangeAsync(perfilesEntity);
                    var lstPerdilesAdd = perfiles.Select(s => new RelUsuariosPerfiles
                    {
                        IdPerfil = s.IdPerfil,
                        Userid = userid
                    }).ToList();

                    await repo.AddARangeAsync(lstPerdilesAdd);
                    await uow.CommitAsync();

                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }

            }
        }

        public async Task<List<PermisoDTO>> GetPermisosForUserAsync(string userid)
        {
            List<PermisoDTO> result = new List<PermisoDTO>();
            var uof = _uowFactory.GetUnitOfWork();
            var repoPermisos = new PermisosRepository(uof);

            var lstPermisosEntity = await repoPermisos.GetPermisosForUser(userid).ToListAsync();
            result = _mapper.Map<List<PermisoDTO>>(lstPermisosEntity);

            return result;
        }
        public async Task<List<PerfilDTO>> GetPerfilesForUserAsync(string userid)
        {
            List<PerfilDTO> result = new List<PerfilDTO>();
            var uof = _uowFactory.GetUnitOfWork();
            var repoUsuariosPerfiles = new RelUsuariosPerfilesRepository(uof);

            var lstPerfilesEntity = await repoUsuariosPerfiles.Where(x=> x.Userid == userid).Select(s=> s.IdPerfilNavigation).ToListAsync();
            result = _mapper.Map<List<PerfilDTO>>(lstPerfilesEntity);

            return result;
        }

        public async Task<List<EmpresaDTO>> GetUsuariosVinculados(string Id)
        {
            List<EmpresaDTO> result = new List<EmpresaDTO>();
            var uof = _uowFactory.GetUnitOfWork();
            var repoEmpresas = new EmpresasRepository(uof);

            var usuEntity = await _signInManager.UserManager.FindByIdAsync(Id);
            var usuarioLogueadoDTO = _mapper.Map<UserProfile, UsuarioDTO>(usuEntity);
            // Si el usuario es ademas empresa tiene que estar dado de alta como empresa.

            //Agrega los usuarios de las empresas a las que podría representar
            var lstEmpresas = await repoEmpresas.Where(x => x.UseridRepresentante == usuarioLogueadoDTO.Id)
                                                     .ToListAsync();

            result = _mapper.Map<List<EmpresaDTO>>(lstEmpresas);
            //--

            return result;

        }
        public async Task<string> SetTokenAsync(string userid, string tokenName = "Login")
        {
            string result = "";
            var usu = await _signInManager.UserManager.FindByIdAsync(userid);
            result = Guid.NewGuid().ToString();
            await _signInManager.UserManager.SetAuthenticationTokenAsync(usu, "Provider", tokenName, result);
            return result;
        }
        public async Task<string> GetTokenAsync(string userid, string tokenName = "Login")
        {
            string result = "";
            var usu = await _signInManager.UserManager.FindByIdAsync(userid);
            result = await _signInManager.UserManager.GetAuthenticationTokenAsync(usu, "Provider", tokenName);
            return result;
        }
        public async Task RemoveTokenAsync(string userid, string tokenName = "Login")
        {
            var usu = await _signInManager.UserManager.FindByIdAsync(userid);
            await _signInManager.UserManager.RemoveAuthenticationTokenAsync(usu, "Provider", tokenName);
        }

        public async Task<CookieDto> GetCookieAuthorization(string UserName, EmpresaDTO empresa = null)
        {
            
            CookieDto result = null!;
                        
            var user = await GetUserByNameAsync(UserName);

            var lstPermisos = await GetPermisosForUserAsync(user.Id);
            var lstPerfiles = await GetPerfilesForUserAsync(user.Id);
            List<string> roles = new List<string>();
            roles.AddRange(lstPermisos.Select(s => s.Codigo).ToList());
            roles.AddRange(lstPerfiles.Select(s => s.Nombre).ToList());

            var userDto = new UsuarioCookieDataDto()
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                NombreyApellido = user.NombreyApellido ?? "",
                Email = user.Email ?? "",
                Roles = roles,
                IdEmpresa = empresa?.IdEmpresa,
                CuitEmpresa = empresa?.CuitEmpresa,
                NombreEmpresa = empresa?.RazonSocial
            };

                
            //get configuration settings
            var cookie_name = _configuration["AppSettings:CookieName"]!;
            var seconds = int.Parse(_configuration["AppSettings:CookieTotalSeconds"]!.ToString());
            var domain = _configuration["AppSettings:CookieDomain"]!;

            var jsonData = JsonConvert.SerializeObject(userDto);

            result = new CookieDto
            {
                Name = cookie_name,
                Value = jsonData,
                Expires = DateTime.Now.AddSeconds(seconds)
            };

            if (!string.IsNullOrEmpty(domain))
                result.Domain = domain;

            
            return result;
        }
    }
}
