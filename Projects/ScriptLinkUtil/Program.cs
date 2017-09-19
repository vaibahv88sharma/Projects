using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;


namespace ScriptLinkUtil
{
    class Program
    {
        static void Main(string[] args)
        {

            // For On-Premise
            ClientContext context = new ClientContext("http://sp01d-bro/sites/search/");
            Web web = context.Web;
            context.Load(web);
            context.ExecuteQuery();
            Console.WriteLine(web.Title);

            // For Office-365
            string url = "https://networkintegration.sharepoint.com/sites/development/";
            string logon = "andrew@365build.com.au";
            string passwordString = "187Ch@lleng3r";

            ClientContext ctx = new ClientContext(url);
            var passWord = new SecureString();
            foreach (char c in passwordString.ToCharArray()) passWord.AppendChar(c);
            ctx.Credentials = new SharePointOnlineCredentials(logon, passWord);
            ClientContext context = ctx;

            ListScriptLinks(context);

            AddScriptLink(context, "~SiteCollection/_catalogs/masterpage/jquery-2.2.4.js", 1300);
            AddScriptLink(context, "~SiteCollection/_catalogs/masterpage/jQueryInjectionNew.js", 1302);
            AddScriptLink(context, "~SiteCollection/_catalogs/masterpage/jQueryInjection.js", 1304);
            AddCSSScriptLink(context, "/_catalogs/masterpage/Watersun", 1306);

            ClearAllScriptLinks(context);
            ListScriptLinks(context);

            AddScriptLink(context, "~SiteCollection/_catalogs/masterpage/jquery-2.2.4.js", 1300);
            AddScriptLink(context, "~SiteCollection/_catalogs/masterpage/jQueryInjectionNew.js", 1302);
            AddScriptLink(context, "~SiteCollection/_catalogs/masterpage/jQueryInjection.js", 1304);
            AddCSSScriptLink(context, "/_catalogs/masterpage/Watersun", 1306);

            RemoveScriptLink(context, "~SiteCollection/_catalogs/masterpage/jquery-2.2.4.js");
            RemoveScriptLink(context, "~SiteCollection/_catalogs/masterpage/jQueryInjectionNew.js");
            RemoveScriptLink(context, "~SiteCollection/_catalogs/masterpage/jQueryInjection.js");
            RemoveScriptLink(context, "~SiteCollection/_catalogs/masterpage/site.css");

            AddScriptLink(context, "~SiteCollection/_catalogs/masterpage/jquery-2.2.4.js", 1300);
            AddScriptLink(context, "~SiteCollection/_catalogs/masterpage/jQueryInjectionNew.js", 1302);
            AddScriptLink(context, "~SiteCollection/_catalogs/masterpage/jQueryInjection.js", 1304);
            AddCSSScriptLink(context, "/_catalogs/masterpage/Watersun", 1306);

            //Console.WriteLine("Press Enter to End");
            //Console.ReadLine();
        }

        /// <summary>
        /// adds a CSS scriptlink to the site 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="file"></param>
        /// <param name="seq"></param>
        private static void AddCSSScriptLink(ClientContext ctx, string file, int seq)
        {
            // Register Custom Action
            var customAction = ctx.Site.UserCustomActions.Add();
            customAction.Location = "ScriptLink";
            customAction.ScriptBlock = @"document.write('<link rel=""stylesheet"" href=""" + ctx.Url + file + @"/site.css"" />');";
            customAction.Sequence = seq;
            customAction.Update();
            ctx.ExecuteQuery();

            Console.WriteLine("ScriptLink Added : {0}", file);
        }
     


        /// <summary>
        /// adds a scriptlink to the site 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="file"></param>
        /// <param name="seq"></param>
        private static void AddScriptLink(ClientContext ctx, string file, int seq)
        {
            // Register Custom Action
            var customAction = ctx.Site.UserCustomActions.Add();
            customAction.Location = "ScriptLink";
            customAction.ScriptSrc = file;
            customAction.Sequence = seq;
            customAction.Update();
            ctx.ExecuteQuery();

            Console.WriteLine("ScriptLink Added : {0}", file);
        }
        
        /// <summary>
        /// remove all customactions from the site
        /// </summary>
        /// <param name="ctx"></param>
        private static void ClearAllScriptLinks(ClientContext ctx)
        {
            var customActions = ctx.Site.UserCustomActions;
            ctx.Load(customActions);
            ctx.ExecuteQuery();
            customActions.Clear();
            ctx.ExecuteQuery();

            Console.WriteLine("All SriptLinks removed");
        }

        /// <summary>
        /// list the scriptlinks on the site
        /// </summary>
        /// <param name="ctx"></param>
        private static void ListScriptLinks(ClientContext ctx)
        {
            var customActions = ctx.Site.UserCustomActions;
            ctx.Load(customActions);
            ctx.ExecuteQuery();
            
            foreach(UserCustomAction ua in customActions)
            {
                if (string.Compare(ua.Location, "ScriptLink", true) == 0)
                {
                    Console.WriteLine("Script Source : {0}, Sequence : {1}", ua.ScriptSrc, ua.Sequence);
                }
            }

            if(customActions.Count == 0)
            {
                Console.WriteLine("No ScriptLinks found for {0}", ctx.Url);
            }
        }
 

        /// <summary>
        /// remove a scriptlink matching script source
        /// </summary>
        /// <param name="ctx"></param>
        private static void RemoveScriptLink(ClientContext ctx, string scriptsource)
        {
            var customActions = ctx.Site.UserCustomActions;
            ctx.Load(customActions);
            ctx.ExecuteQuery();

            foreach (UserCustomAction ua in customActions)
            {
                if (string.Compare(ua.ScriptSrc, scriptsource, true) == 0)
                {
                    Console.WriteLine("Removing Script Src : {0}, Sequence : {1}", ua.ScriptSrc, ua.Sequence);
                    ua.DeleteObject();
                }
            }

            if(ctx.HasPendingRequest)
            {
                ctx.ExecuteQuery();
            }
        }


        private static SecureString GetPasswordFromConsoleInput()
        {
            ConsoleKeyInfo info;

            //Get the user's password as a SecureString
            SecureString securePassword = new SecureString();
            do
            {
                info = Console.ReadKey(true);
                if (info.Key != ConsoleKey.Enter)
                {
                    securePassword.AppendChar(info.KeyChar);
                }
            }
            while (info.Key != ConsoleKey.Enter);
            return securePassword;
        }
    }
}



