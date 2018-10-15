namespace SalesApi.Controllers
{
    using System;
    using System.IO;
    using System.Web.Http;
    using Helpers;
    using Newtonsoft.Json.Linq;
    using Sales.Common.Models;

    [RoutePrefix("api/Users")]
    public class UsersController : ApiController
    {
        //aqui recikbo el userrequis desde el from ya armado:
        public IHttpActionResult PostUser(UserRequest userRequest)
        {
            //aqui valido si hay foto, y si hay la guado en la carpeta del usurio:
            if (userRequest.ImageArray != null && userRequest.ImageArray.Length > 0)
            {
                var stream = new MemoryStream(userRequest.ImageArray);
                //un Guid es un nombre aleatorio:
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "~/Content/Users";
                var fullPath = $"{folder}/{file}";
                var response = FilesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    userRequest.ImagePath = fullPath;
                }
            }

            //pilas conla respuesta, hay que preguntas, si toes satisfactorio,
            //por que de lo contrario siempre envia un ok:

            var answer = UsersHelper.CreateUserASP(userRequest);

            if (answer.IsSuccess)
            {
              return Ok(answer);

            }

            return BadRequest(answer.Message);

        }

        [HttpPost]
        //[Authorize]
        [Route("GetUser")]
        public IHttpActionResult GetUser(JObject form)
        {
            try
            {
                var email = string.Empty;
                dynamic jsonObject = form;

                try
                {
                    email = jsonObject.Email.Value;
                }
                catch
                {
                    return BadRequest("Incorrect call.");
                }

                var user = UsersHelper.GetUserASP(email);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("LoginFacebook")]
        public IHttpActionResult LoginFacebook(FacebookResponse profile)
        {
            var user = UsersHelper.GetUserASP(profile.Id);
            if (user != null)
            {
                return Ok(true);
            }

            var userRequest = new UserRequest
            {
                EMail = profile.Id,
                FirstName = profile.FirstName,
                ImagePath = profile.Picture.Data.Url,
                LastName = profile.LastName,
                Password = profile.Id,
            };

            var answer = UsersHelper.CreateUserASP(userRequest);

            if (answer.IsSuccess)
            {
                return Ok(answer);

            }

            return BadRequest(answer.Message);
        }

    }

}
