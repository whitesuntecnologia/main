using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace StaticClass
{
    public static class Functions
    {
        public static bool ValidateCUIT(string cuit)
        {
            bool result = false;
            int verificador;
            int resultado = 0;
            string cuit_nro = cuit.Replace("-", string.Empty);
            string codes = "6789456789";
            long cuit_long = 0;

            if (long.TryParse(cuit_nro, out cuit_long))
            {
                verificador = int.Parse(cuit_nro[cuit_nro.Length - 1].ToString());
                int x = 0;

                while (x < 10)
                {
                    int digitoValidador = int.Parse(codes.Substring((x), 1));
                    int digito = int.Parse(cuit_nro.Substring((x), 1));
                    int digitoValidacion = digitoValidador * digito;
                    resultado += digitoValidacion;
                    x++;
                }

                resultado = resultado % 11;

                if (resultado == verificador)
                {
                    result = true;
                }
            }

            return result;
        }
        public static string GetErrorMessage(Exception ex)
        {
            string ret = "";

            if (ex.InnerException != null)
                ret = GetErrorMessage(ex.InnerException);
            else
                ret = ex.Message;

            return ret;
        }
        public static bool IsAuthenticated(ClaimsPrincipal? _user, bool refreshExpiration = true)
        {
            bool result = false;

            var StartProcess = DateTime.Now;
            int EllapsedMiliseconds = 0;

            if (_user != null && _user.HasClaim(c => c.Type == ClaimTypes.Expiration))
            {
                var claim = _user.FindFirst(ClaimTypes.Expiration);
                var now = DateTime.Now;
                var cultureInfo = new CultureInfo("es");
                DateTime? expiration = null;
                if (claim != null)
                    expiration = DateTime.ParseExact(claim.Value, "yyyyMMddHHmmss", cultureInfo);

                if (now <= expiration)
                {
                    if (refreshExpiration)
                    {
                        var identity = _user.Identity as ClaimsIdentity;
                        expiration = now.AddSeconds(StaticClass.Constants.SessionJsTimeout);
                        identity?.RemoveClaim(claim);
                        identity?.AddClaim(new Claim(ClaimTypes.Expiration, expiration.Value.ToString("yyyyMMddHHmmss")));
                    }
                    result = true;
                }

                EllapsedMiliseconds = Convert.ToInt32((DateTime.Now - StartProcess).TotalMilliseconds);
                System.Diagnostics.Debug.WriteLine($"{EllapsedMiliseconds}  - {expiration}");
            }

            return result;
        }
        public static void CopyProperties(object source, object destination)
        {
            if (source == null || destination == null)
            {
                throw new ArgumentNullException("Los objetos de origen y destino no pueden ser nulos.");
            }

            Type sourceType = source.GetType();
            Type destinationType = destination.GetType();

            PropertyInfo[] sourceProperties = sourceType.GetProperties();
            PropertyInfo[] destinationProperties = destinationType.GetProperties();

            foreach (var sourceProperty in sourceProperties)
            {
                foreach (var destinationProperty in destinationProperties)
                {
                    if (sourceProperty.Name == destinationProperty.Name &&
                        sourceProperty.PropertyType == destinationProperty.PropertyType &&
                        destinationProperty.CanWrite)
                    {
                        destinationProperty.SetValue(destination, sourceProperty.GetValue(source));
                        break;
                    }
                }
            }
        }
        public static string GetConceptoFromCoef(decimal coeficienteConceptual)
        {
            string result = "";
            if (coeficienteConceptual >= 0 && coeficienteConceptual <= 0.5m)
                result = "Malo";
            else if (coeficienteConceptual > 0.5m && coeficienteConceptual <= 0.8m)
                result = "Regular";
            else if (coeficienteConceptual > 0.8m && coeficienteConceptual <= 1.10m)
                result = "Bueno";
            else if (coeficienteConceptual > 1.10m)
                result = "Muy Bueno";

            return result;
        }
        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            using var rng = RandomNumberGenerator.Create();
            var result = new char[length];
            var buffer = new byte[sizeof(uint)];

            for (int i = 0; i < length; i++)
            {
                rng.GetBytes(buffer);
                uint num = BitConverter.ToUInt32(buffer, 0);
                result[i] = chars[(int)(num % (uint)chars.Length)];
            }

            return new string(result);
        }
        public static int? ConvertPeriodoToNumber(string Periodo)
        {
            int? result = null;
            if (!string.IsNullOrWhiteSpace(Periodo) && Periodo.Length == 7 )
            {
                result = int.Parse(Periodo.Substring(3, 4) + Periodo.Substring(0, 2));
            }

            return result;

        }
    }
}
