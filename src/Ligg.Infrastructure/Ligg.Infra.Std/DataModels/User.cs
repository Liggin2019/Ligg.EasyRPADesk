
using Newtonsoft.Json;
using System.ComponentModel;
using System;

namespace Ligg.Infrastructure.DataModels
{
    public class UserInfo
    {
        public string Id { get; set; }
        public string ActorId { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public string ThumbnailPostfix { get; set; }


    }


}
