namespace Sales.Common.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [Display(Name = "Image")]
        public string ImagePath { get; set; }

        //relacion con tabla product, lado uno
        [JsonIgnore]//es para que este campo no sea tenido encuenta en la desarializacion
        public virtual ICollection<Product> Products { get; set; }

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(this.ImagePath))
                {
                    return "noproduct";
                }

                //return $"https://salesbackend.azurewebsites.net{this.ImagePath.Substring(1)}";
                //return $"http://192.168.0.11:54268/{ImagePath.Substring(1)}";
                return $"http://localhost:57100/{ImagePath.Substring(1)}";
            }
        }

    }
}
