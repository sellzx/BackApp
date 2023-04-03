using Amazon.DynamoDBv2.DataModel;
using BackApp.Models.AWS.DynamoDBEntities;
using BackApp.Models.Input;
using BackApp.Models.Output;
using BackApp.Services.DynamoDB.Repository;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackApp.Services.DynamoDB
{
    public class LoginService : DynamoDbRepository<LoginServiceEntity>
    {
        public LoginService()
        : base(typeof(LoginServiceEntity), "Users-table", true)
        {
        }

        public async Task<RegisterOutputModel> RegisterUserAsync(RegisterInputModel registerInputModel)
        {
            var result = new RegisterOutputModel();
            var user = registerInputModel.Email.ToLower();
            var getUser = await GetAsync(user);
            //Se chequea existencia de usuario
            if (getUser != null)
            {
                result.Success = false;
                result.Message = $"Ya existe un usuario con correo {registerInputModel.Email}";
                return result;
            }
            else
            {
                var newUser = new LoginServiceEntity()
                {
                    EmailAdress = user,
                    Password = registerInputModel.Password,
                    Name = registerInputModel.Name,
                    LastName = registerInputModel.LastName,
                    Friends = new List<string>(),
                    Requests = new List<string>()
                };
                result.Success = await SaveAsync(newUser);
                result.Message = result.Success ? "Usuario registrado con exito" : "Ocurrio un problema";
            }
            return result;
        }

        public async Task<FriendRequestOutputModel> AcceptFriends(RequestInputModel model)
        {
            var result = new FriendRequestOutputModel();
            var getAccepter = await GetAsync(model.Requested);
            var getAccepted = await GetAsync(model.Requester);

            if (getAccepted.Friends == null) getAccepted.Friends = new List<string>();
            getAccepted.Friends.Add(getAccepter.EmailAdress);
            if (getAccepter.Friends == null) getAccepter.Friends = new List<string>();
            getAccepter.Friends.Add(getAccepted.EmailAdress);
            getAccepter.Requests.Remove(model.Requester);
            if (getAccepter.Requests.Count == 0)
            {
                getAccepter.Requests = new List<string> { "" };
            }

            result.Message = $"Se añadio a {model.Requester} a tu lista de amigos!";
            var listSave = new List<LoginServiceEntity> { getAccepted, getAccepter };
            result.Success = await SaveBatchAsync(listSave);
            return result;
        }

        public async Task<FriendRequestOutputModel> DeleteFriend(RequestInputModel model)
        {
            var result = new FriendRequestOutputModel();
            var getAccepter = await GetAsync(model.Requested);
            var getAccepted = await GetAsync(model.Requester);

            getAccepted.Friends.Remove(getAccepter.EmailAdress);
            getAccepter.Friends.Remove(getAccepted.EmailAdress);

            result.Message = $"Se quito a {model.Requested} de tu lista de amigos!";
            var listSave = new List<LoginServiceEntity> { getAccepted, getAccepter };
            result.Success = await SaveBatchAsync(listSave);
            return result;
        }

        public async Task<FriendRequestOutputModel> DeclineRequest(RequestInputModel model)
        {
            var result = new FriendRequestOutputModel();
            var getAccepter = await GetAsync(model.Requested);
            getAccepter.Requests = getAccepter.Requests.Where(u => !u.Contains(model.Requester)).Select(u => u).ToList();
            if (getAccepter.Requests.Count == 0)
            {
                getAccepter.Requests = new List<string> { "" };
            }
            result.Message = $"Se ha quitado a {model.Requester} de tus solicitudes";
            result.Success = await SaveAsync(getAccepter);
            return result;
        }

        public async Task<List<string>> GetFriends(string user)
        {
            var userInfo = await GetAsync(user);

            return userInfo.Friends == null ? new List<string>() : userInfo.Friends;
        }

        public async Task<List<string>> GetRequests(string user)
        {
            var userInfo = await GetAsync(user);

            return userInfo.Requests == null ? new List<string>() : userInfo.Requests;
        }

        public async Task<FriendRequestOutputModel> FriendRequest(RequestInputModel model)
        {
            var result = new FriendRequestOutputModel();
            if (model.Requested == model.Requester)
            {
                result.Message = $"No se puede añadir a si mismo {model.Requested}.";
                return result;
            }
            var getRequested = await GetAsync(model.Requested);

            if (getRequested == null)
            {
                result.Message = $"No existe el usuario {model.Requested} en los registros.";
                return result;
            };
            if (getRequested.Requests == null || (getRequested.Requests.Count == 1 && getRequested.Requests[0] == ""))
            {
                getRequested.Requests = new List<string>();
            }
            foreach (var item in getRequested.Requests)
            {
                if (item.Contains(model.Requester))
                {
                    result.Message = $"Ya se ha enviado solicitud de amistad a {model.Requested}";
                    result.Success = false;
                    return result;
                }
            }
            getRequested.Requests.Add(model.Requester);

            result.Message = $"Se ha enviado solicitud de amistad a {model.Requested}";
            result.Success = await SaveAsync(getRequested);

            return result;
        }

        public async Task<LoginOutputModel> CheckUser(string email, string password)
        {
            var result = new LoginOutputModel();
            var getUser = await GetAsync(email.ToLower());
            if (getUser != null)
            {
                result.Success = password == getUser.Password;
                result.Message = result.Success ? "Login Correcto" : "Contraseña Incorrecta";
            }
            else
            {
                result.Message = "Usuario no existe";
            }

            return result;
        }
    }
}
