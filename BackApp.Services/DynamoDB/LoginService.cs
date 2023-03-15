﻿using Amazon.DynamoDBv2.DataModel;
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

            getAccepted.Friends.Add(getAccepter.EmailAdress);
            getAccepter.Friends.Add(getAccepted.EmailAdress);
            getAccepter.Requests.Remove(model.Requester);

            result.Message = $"Se añadio a {model.Requester} a tu lista de amigos!";
            await SaveAsync(getAccepted);
            result.Success = await SaveAsync(getAccepter);
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
            await SaveAsync(getAccepted);
            result.Success = await SaveAsync(getAccepter);
            return result;
        }

        public async Task<FriendRequestOutputModel> DeclineRequest(RequestInputModel model)
        {
            var result = new FriendRequestOutputModel();
            var getAccepter = await GetAsync(model.Requested);

            getAccepter.Requests.Remove(model.Requester);

            result.Message = $"Se ha quitado a {model.Requester} de tus solicitudes";
            result.Success = await SaveAsync(getAccepter);
            return result;
        }

        public async Task<List<string>> GetFriends(string user)
        {
            var userInfo = await GetAsync(user);

            return userInfo.Friends;
        }

        public async Task<FriendRequestOutputModel> FriendRequest(RequestInputModel model)
        {
            var result = new FriendRequestOutputModel();
            var getRequested = await GetAsync(model.Requested);

            if (getRequested == null)
            {
                result.Message = $"No existe el usuario {model.Requested} en los registros.";
                return result;
            };

            if (getRequested.Requests.Contains(model.Requester))
            {
                result.Message = $"Ya se ha enviado solicitud de amistad a {model.Requested}";
                result.Success = false;
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
