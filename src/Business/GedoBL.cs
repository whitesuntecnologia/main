using Business.Interfaces;
using DataTransferObject;
using DocumentFormat.OpenXml.Drawing;
using Newtonsoft.Json;
using StaticClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork;

namespace Business
{
    public class GedoBL : IGedoBL
    {
        private readonly IParametrosBL _parametrosBL;
        public GedoBL(IParametrosBL parametrosBL)
        {
            _parametrosBL = parametrosBL;
        }

        public async Task<byte[]> ObtenerPdfDocumento(string NroDocuemento)
        {
            byte[] result = null!;
            var urlGde = await _parametrosBL.GetParametroCharAsync("GDE.REST.Host");
            var usuarioGdeServicio = await _parametrosBL.GetParametroCharAsync("GDE.REST.Usuario");
            var passwordGdeServicio = await _parametrosBL.GetParametroCharAsync("GDE.REST.Password");
            var nroInstitucion = (int) await _parametrosBL.GetParametroNumAsync("GDE.REST.NroInstitucion");
            var usuarioGdeConsulta = await _parametrosBL.GetParametroCharAsync("SADE.UsuarioDirector");


            string uriString = $"{urlGde}/apigdegx/rest/WSconsultarDocumentoPDF";
            var httpClientHandler = new HttpClientHandler
            {
                SslProtocols = System.Security.Authentication.SslProtocols.Tls12 |
                    System.Security.Authentication.SslProtocols.Tls11 |
                    System.Security.Authentication.SslProtocols.Tls
            };
            HttpClient client = new HttpClient(httpClientHandler);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uriString);

            try
            {

                var clsBody = new
                {
                    NroInstitucion = nroInstitucion,
                    Usuario = usuarioGdeServicio,
                    Clave = passwordGdeServicio,
                    NumeroGDE = NroDocuemento,
                    UsuarioGDE = usuarioGdeConsulta
                };

                
                request.Content = new StringContent(JsonConvert.SerializeObject(clsBody), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.SendAsync(request);
                

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var dto = JsonConvert.DeserializeObject<ResponseConsultarDocumentoPdfDto>(responseContent);
                    if(dto != null && !string.IsNullOrWhiteSpace(dto.ErrorOut))
                    {
                        throw new Exception($"Error devuelto por GDE al consultar el pdf {NroDocuemento}: {dto.ErrorOut}");
                    }
                    else
                        result = dto?.RespuestaDocPDF ?? Array.Empty<byte>();
                    
                }
                else
                {
                    throw new ArgumentException($"Error al consultar el servicio de GDE para obtener el pdf: {response.Content}");
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                client.Dispose();
            }

            return result;

        }
    }
}
