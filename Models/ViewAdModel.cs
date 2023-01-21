using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace _2017VisualStudioMVC.Models
{
    public class ViewAdModel
    {
        public int pro_id { get; set; }
        public string pro_name { get; set; }
        public string pro_image { get; set; }
        public Nullable<int> pro_price { get; set; }
        public string pro_des { get; set; }
        public Nullable<int> pro_fk_cat { get; set; }
        public Nullable<int> pro_fk_user { get; set; }

        public string cat_name { get; set; }
        public Nullable<int> cat_fk_ad { get; set; }

        public string u_name { get; set; }
        public string u_email { get; set; }
        public string u_image { get; set; }
        public string u_contact { get; set; }




    }
}