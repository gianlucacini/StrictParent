using NetFwTypeLib;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unplug.BusinessLayer
{
    public class FirewallRule
    {
        public static readonly String Name = "Unplug Rule";

        /// <summary>
        /// Method that constructs a rule in the firewall that blocks all connections
        /// </summary>
        /// <returns></returns>
        public static INetFwRule Rule
        {
            get
            {
                INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
                firewallRule.Description = "This rule blocks internet connection.";
                firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
                firewallRule.Enabled = true;
                firewallRule.InterfaceTypes = "All";
                firewallRule.Name = Name;
                return firewallRule;
            }
        }

        /// <summary>
        /// Add firewall rule that blocks all connections
        /// </summary>
        public static void DenyConnection(Boolean writeToLog)
        {
            if (writeToLog)
                Log.Information("DenyConnection called");

            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            firewallPolicy.Rules.Add(Rule);
        }

        /// <summary>
        /// Delete previously created firewall rule that blocks all connections
        /// </summary>
        public static void AllowConnection(Boolean writeToLog)
        {
            if (writeToLog)
                Log.Information("AllowConnection called");

            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            firewallPolicy.Rules.Remove(Name);
        }

        private static INetFwPolicy2 FirewallMgr
        {
            get
            {
                Type netFwPolicy2Type = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
                INetFwPolicy2 mgr = (INetFwPolicy2)Activator.CreateInstance(netFwPolicy2Type);
                return mgr;
            }
        }

        public static void EnableFirewallIfDown()
        {
            int currentProfile = FirewallMgr.CurrentProfileTypes;

            bool firewallEnabled = FirewallMgr.FirewallEnabled[(NET_FW_PROFILE_TYPE2_)currentProfile];

            if (firewallEnabled == false)
            {
                Log.Information("Firewall is not enabled. Enabling firewall with profile " + currentProfile);

                FirewallMgr.set_FirewallEnabled((NET_FW_PROFILE_TYPE2_)currentProfile, true);
                
                //TODO Maybe check a second time?
            }
        }
    }
}
