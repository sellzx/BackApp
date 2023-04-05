using BackApp.Models.AWS.DynamoDBEntities;
using BackApp.Models.Input;
using BackApp.Models.Output;
using BackApp.Services.DynamoDB.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BackApp.Services.DynamoDB
{
    public class PostService : DynamoDbRepository<PostOwnerEntity>
    {
        public PostService()
        : base(typeof(PostOwnerEntity), "Posts-table", true)
        {
        }

        public async Task<PostOutputModel> NewPost(PostInputModel postModel)
        {
            var result = new PostOutputModel();
            postModel.Date = DateTime.Now;

            var newComment = await GetAsync(postModel.User, postModel.Url);
            foreach (var coment in newComment.Coments)
            {
                if (coment.UserName == null)
                {
                    coment.UserName = postModel.User;
                    coment.Date = postModel.Date;
                    coment.Post = postModel.Post;
                    break;
                }
                else
                {
                    var newCom = new Coments
                    {
                        UserName = postModel.User,
                        Date = postModel.Date,
                        Post = postModel.Post
                    };
                    newComment.Coments.Add(newCom);
                    break;
                }
            }

            result.Success = await SaveAsync(newComment);
            result.Message = result.Success ? "Post exitoso" : "Ocurrio un problema" ;
            return result;
        }
    }
}
