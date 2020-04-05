using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens.Saml2;
using SAMLPortal.Misc;
using SAMLPortal.Models;

namespace SAMLPortal.Controllers
{
    [Authorize]
    [Route("Auth")]
    public class AuthController : Controller
    {
        private readonly Models.IAuthenticationService _authService;
        private readonly Saml2Configuration _samlConfig;


        public AuthController(Models.IAuthenticationService authService)
        {
            _authService = authService;
            _samlConfig = new Saml2Configuration
            {
                Issuer = GlobalSettings.Get("CONFIG_CompanyName") + "-MAIN-ISSUER",
                SingleSignOnDestination = new Uri(GlobalSettings.Get("CONFIG_URL") + "/Auth/Login"),
                SingleLogoutDestination = new Uri(GlobalSettings.Get("CONFIG_URL") + "/Auth/Logout"),
                SigningCertificate = GlobalSettings._signingCertificate,
                CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust,
                RevocationMode = X509RevocationMode.NoCheck
            };
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IActionResult> Login()
        {
            LoginViewModel emptyLogin = new LoginViewModel();
            emptyLogin.Username = "";
            emptyLogin.Password = "";

            return View(emptyLogin);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    var user = _authService.Login(model.Username, model.Password);
                    if (null != user)
                    {
                        var userClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.DisplayName),
                            new Claim(ClaimTypes.NameIdentifier, user.Username),
                            new Claim(ClaimTypes.Email, user.Email)
                        };

                        foreach (var membership in user.Memberships)
                        {
                            userClaims.Add(new Claim("membership", membership));
                        }


                        List<string> rolesToClaim = new List<string>();
                        if (user.IsAdmin)
                        {
                            rolesToClaim.Add(UserRoles.Administrator);
                            rolesToClaim.Add(UserRoles.User);
                        }
                        else
                        {
                            rolesToClaim.Add(UserRoles.User);
                        }

                        foreach (var role in rolesToClaim)
                        {
                            userClaims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        var principal = new ClaimsPrincipal(new ClaimsIdentity(userClaims, _authService.GetType().Name));
                        await HttpContext.SignInAsync("SAMLPortal", principal);
                        return Redirect("/");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(model);
        }

        [Route("Logout")]
        public IActionResult Logout()
        {
            var requestBinding = new Saml2PostBinding();
            //var verifiedApp = ValidateApp(ReadAppFromRequest(requestBinding));
            var verifiedApp = new App();

            // Logout from other app via SAML request
            if (verifiedApp != null)
            {
                //var saml2LogoutRequest = new Saml2LogoutRequest(_samlConfig);
                //X509Certificate2 signatureValidationCertificate = new X509Certificate2();
                //signatureValidationCertificate.Import(verifiedApp.SignatureValidationCertificate);
                //saml2LogoutRequest.SignatureValidationCertificates.Append(signatureValidationCertificate);

                try
                {
                    //requestBinding.Unbind(Request.ToGenericHttpRequest(), saml2LogoutRequest);
                    HttpContext.SignOutAsync("SAMLPortal");

                    return Redirect("/");
                    //return LogoutResponse(saml2LogoutRequest.Id, Saml2StatusCodes.Success, requestBinding.RelayState, saml2LogoutRequest.SessionIndex, verifiedApp);
                }
                catch (Exception ex)
                {
#if DEBUG
                    //Debug.WriteLine($"Saml 2.0 Logout Request error: {ex.ToString()}\nSaml Logout Request: '{saml2LogoutRequest.XmlDocument?.OuterXml}'");
#endif
                    HttpContext.SignOutAsync("SAMLPortal");
                    return Redirect("/");
                    //return LogoutResponse(saml2LogoutRequest.Id, Saml2StatusCodes.Responder, requestBinding.RelayState, saml2LogoutRequest.SessionIndex, verifiedApp);
                }
            }
            else
            {
                HttpContext.SignOutAsync("SAMLPortal");
                return Redirect("/");
            }
        }

        [Route("StartRequest")]
        public IActionResult StartRequest()
        {
            var requestBinding = new Saml2RedirectBinding();
            var requestedApp = ReadAppFromRequest(requestBinding);
            var verifiedApp = ValidateApp(requestedApp);

            var saml2AuthnRequest = new Saml2AuthnRequest(_samlConfig);

            try
            {
                requestBinding.Unbind(Request.ToGenericHttpRequest(), saml2AuthnRequest);
                var sessionIndex = Guid.NewGuid().ToString();

                return LoginResponse(saml2AuthnRequest.Id, Saml2StatusCodes.Success, requestBinding.RelayState, verifiedApp, sessionIndex, User.Claims);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Saml 2.0 Authn Request error: {ex.ToString()}\nSaml Auth Request: '{saml2AuthnRequest.XmlDocument?.OuterXml}'\nQuery String: {Request.QueryString}");
                Debug.WriteLine(ex.StackTrace);
#endif

                return LoginResponse(saml2AuthnRequest.Id, Saml2StatusCodes.Responder, requestBinding.RelayState, verifiedApp);
            }
        }


        /// <summary>
        /// Read the unique app issuer from the request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="binding"></param>
        /// <returns></returns>
        private string ReadAppFromRequest<T>(Saml2Binding<T> binding)
        {
            var test = new Saml2AuthnRequest(_samlConfig);
            return binding.ReadSamlRequest(Request.ToGenericHttpRequest(), new Saml2AuthnRequest(_samlConfig))?.Issuer;
        }

        /// <summary>
        /// Search in the database the App corresponding to the given issuer.
        /// </summary>
        /// <param name="issuer"></param>
        /// <returns></returns>
        private App ValidateApp(string issuer)
        {
            SAMLPortalContext context = new SAMLPortalContext();

            return context.App.Where(app => app.Issuer != null && app.Issuer.Equals(issuer, StringComparison.InvariantCultureIgnoreCase)).Single();
        }


        private IActionResult LoginResponse(Saml2Id inResponseTo, Saml2StatusCodes status, string relayState, App app, string sessionIndex = null, IEnumerable<Claim> claims = null)
        {
            var responseBinding = new Saml2PostBinding();
            responseBinding.RelayState = relayState;

            var saml2AuthnResponse = new Saml2AuthnResponse(_samlConfig)
            {
                InResponseTo = inResponseTo,
                Status = status,
                Destination = app.SingleSignOnDestination
            };

            if(status == Saml2StatusCodes.Success && claims != null)
            {
                saml2AuthnResponse.SessionIndex = sessionIndex;

                var claimsIdentity = new ClaimsIdentity(claims);
                saml2AuthnResponse.NameId = new Saml2NameIdentifier(claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).Single(), NameIdentifierFormats.Persistent);
                saml2AuthnResponse.ClaimsIdentity = claimsIdentity;

                var token = saml2AuthnResponse.CreateSecurityToken(app.Issuer, subjectConfirmationLifetime: 5, issuedTokenLifetime: 60);
            }

            return responseBinding.Bind(saml2AuthnResponse).ToActionResult();
        }

        private IActionResult LogoutResponse(Saml2Id inResponseTo, Saml2StatusCodes status, string relayState, string sessionIndex, App app)
        {
            var responseBinding = new Saml2PostBinding();
            responseBinding.RelayState = relayState;

            var saml2LogoutResponse = new Saml2LogoutResponse(_samlConfig)
            {
                InResponseTo = inResponseTo,
                Status = status,
                Destination = app.SingleLogoutResponseDestination,
                SessionIndex = sessionIndex
            };

            return responseBinding.Bind(saml2LogoutResponse).ToActionResult();
        }

        //[Route("AutoDetect")]
        //[HttpGet]
        //public IActionResult AutoDetect(LoginViewModel model)
        //{
        //    SAMLPortalContext context = new SAMLPortalContext();

        //    var entityDescriptor = new EntityDescriptor();
        //    entityDescriptor.ReadSPSsoDescriptorFromUrl(new Uri("http://localhost:8090/users/auth/saml/metadata"));

        //    if (entityDescriptor.SPSsoDescriptor != null)
        //    {

        //        App gitlabApp = new App();
        //        gitlabApp.Description = "The best GitHub alternative";
        //        gitlabApp.Enabled = true;
        //        gitlabApp.Issuer = "GITLAB-ISSUER";
        //        gitlabApp.MetadataURL = "http://localhost:8090/users/auth/saml/metadata";
        //        gitlabApp.Name = "Gitlab";
        //        gitlabApp.SignatureValidationCertificate = "";
        //        gitlabApp.SingleLogoutResponseDestination = new Uri("http://localhost/logout");
        //        gitlabApp.SingleSignOnDestination = entityDescriptor.SPSsoDescriptor.AssertionConsumerServices.First().Location;

        //        context.Add(gitlabApp);
        //        context.SaveChanges();
        //    }


        //    return Redirect("/");

        //}


        //Future code to be placed in another controller. Used to get informations from MetadataURL and populate our App model on request from the admin page
        //if (string.IsNullOrEmpty(rp.Issuer))
        //{
        //    var entityDescriptor = new EntityDescriptor();
        //    entityDescriptor.ReadSPSsoDescriptorFromUrl(new Uri(rp.Metadata));
        //    if (entityDescriptor.SPSsoDescriptor != null)
        //    {
        //        rp.Issuer = entityDescriptor.EntityId;
        //        rp.SingleSignOnDestination = entityDescriptor.SPSsoDescriptor.AssertionConsumerServices.First().Location;
        //        var singleLogoutService = entityDescriptor.SPSsoDescriptor.SingleLogoutServices.First();
        //        rp.SingleLogoutResponseDestination = singleLogoutService.ResponseLocation ?? singleLogoutService.Location;
        //        rp.SignatureValidationCertificate = entityDescriptor.SPSsoDescriptor.SigningCertificates.First();
        //    }
        //    else
        //    {
        //        throw new Exception($"SPSsoDescriptor not loaded from metadata '{rp.Metadata}'.");
        //    }
        //}
    }
}