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
            var user = registerInputModel.EmailAdress.ToLower();
            var getUser = await GetAsync(user);
            //Se chequea existencia de usuario
            if (getUser != null)
            {
                result.Success = false;
                result.Message = $"Ya existe un usuario con correo {registerInputModel.EmailAdress}";
                return result;
            }
            else
            {
                var newUser = new LoginServiceEntity()
                {
                    EmailAdress = user,
                    Password = registerInputModel.Password,
                    Name = registerInputModel.Name,
                    LastName = registerInputModel.LastName
                };
                result.Success = await SaveAsync(newUser);
                result.Message = result.Success ? "Usuario registrado con exito" : "Ocurrio un problema";
            }
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
