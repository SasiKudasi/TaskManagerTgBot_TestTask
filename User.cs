using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerTgBot
{
    public class User
    {
        public long Id { get; set; }
        public string Message { get; set; }


        public User(long id, string message)
        {

            Id = id;
            Message = message;
        }

    }
}
