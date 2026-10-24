// Made by PapaGordon
using NetFwTypeLib;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace WindowsGSM
{
    partial class WindowsFirewall
    {
        internal const string PortRuleGroup = "WindowsGSM Auto Firewall";

        internal static INetFwRule CreatePortRule(Functions.ServerFirewall.PortRule settings)
        {
            var rule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            rule.Name = settings.Name;
            rule.Grouping = PortRuleGroup;
            rule.Description = "Automatic game-server port rule managed by WindowsGSM.";
            rule.Protocol = settings.Protocol;
            rule.LocalPorts = settings.LocalPorts;
            rule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
            rule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            rule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL;
            rule.Enabled = true;
            return rule;
        }

        private static INetFwRule FindPortRule(INetFwRules rules, string name)
        {
            try { return rules.Item(name); }
            catch (Exception e) when ((uint)e.HResult == 0x80070002) { return null; }
        }

        internal async Task<bool> SetPortEnabled(Functions.ServerFirewall.PortRule settings, bool enabled)
        {
            return await Task.Run(() =>
            {
                Error = null;
                try
                {
                    var policy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                    var rules = policy.Rules;
                    var existing = FindPortRule(rules, settings.Name);
                    if (existing != null && existing.Grouping != PortRuleGroup)
                    {
                        Error = "A rule with this name already exists outside WindowsGSM: " + settings.Name;
                        return false;
                    }
                    if (!enabled)
                    {
                        if (existing != null) rules.Remove(settings.Name);
                    }
                    else if (existing == null)
                        rules.Add(CreatePortRule(settings));
                    else
                    {
                        existing.Protocol = settings.Protocol;
                        existing.LocalPorts = settings.LocalPorts;
                        existing.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
                        existing.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                        existing.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL;
                        existing.Enabled = true;
                    }
                    policy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                    var current = FindPortRule(policy.Rules, settings.Name);
                    if (!enabled && current == null) return true;
                    if (enabled && current != null && current.Enabled && current.Grouping == PortRuleGroup
                        && current.Protocol == settings.Protocol && current.LocalPorts == settings.LocalPorts
                        && current.Direction == NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN
                        && current.Action == NET_FW_ACTION_.NET_FW_ACTION_ALLOW) return true;
                    Error = "The automatic port rule could not be changed: " + settings.Name;
                    return false;
                }
                catch (Exception e) { Error = e.Message; return false; }
            });
        }
    }
}
