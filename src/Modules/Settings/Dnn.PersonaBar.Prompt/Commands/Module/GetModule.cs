﻿using Dnn.PersonaBar.Library.Prompt.Attributes;
using Dnn.PersonaBar.Prompt.Models;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using System.Collections.Generic;
using System.Text;
using Dnn.PersonaBar.Library.Prompt;
using Dnn.PersonaBar.Library.Prompt.Models;
namespace Dnn.PersonaBar.Prompt.Commands.Module
{
    [ConsoleCommand("get-module", "Gets module information for module specified", new[] { "id" })]
    public class GetModule : ConsoleCommandBase
    {

        private const string FlagId = "id";

        private const string FlagPageid = "pageid";

        protected int? ModuleId { get; private set; }
        protected int? PageId { get; private set; }

        public override void Init(string[] args, PortalSettings portalSettings, UserInfo userInfo, int activeTabId)
        {
            base.Init(args, portalSettings, userInfo, activeTabId);
            var sbErrors = new StringBuilder();

            if (HasFlag(FlagId))
            {
                var tmpId = 0;
                if (int.TryParse(Flag(FlagId), out tmpId))
                {
                    ModuleId = tmpId;
                }
                else
                {
                    sbErrors.AppendFormat("The --{0} flag must be an integer", FlagId);
                }
            }
            else
            {
                // attempt to get it as the first argument
                if (args.Length >= 2 && !IsFlag(args[1]))
                {
                    var tmpId = 0;
                    if (int.TryParse(args[1], out tmpId))
                    {
                        ModuleId = tmpId;
                    }
                    else
                    {
                        sbErrors.AppendFormat("The Module ID is required. Please use the --{0} flag or pass it as the first argument after the command name", FlagId);
                    }
                }
            }

            if (HasFlag(FlagPageid))
            {
                var tmpId = 0;
                if (int.TryParse(Flag(FlagPageid), out tmpId))
                {
                    PageId = tmpId;
                }
                else
                {
                    sbErrors.AppendFormat("--{0} must be an integer; ", FlagPageid);
                }
            }

            if (ModuleId.HasValue && ModuleId <= 0)
            {
                sbErrors.Append("The Module's ID must be greater than 0");
            }
            ValidationMessage = sbErrors.ToString();
        }

        public override ConsoleResultModel Run()
        {

            if (PageId.HasValue)
            {
                // getting a specific module instance
                var lst = new List<ModuleInstanceModel>();

                var mi = ModuleController.Instance.GetModule((int)ModuleId, (int)PageId, true);
                if (mi != null)
                {
                    lst.Add(ModuleInstanceModel.FromDnnModuleInfo(mi));
                }
                return new ConsoleResultModel(string.Empty) { Data = lst };
            }
            else
            {
                var lst = new List<ModuleInfoModel>();
                var results = ModuleController.Instance.GetAllTabsModulesByModuleID((int)ModuleId);
                if (results != null && results.Count > 0)
                {
                    lst.Add(ModuleInfoModel.FromDnnModuleInfo((ModuleInfo)results[0]));
                }
                else
                {
                    return new ConsoleResultModel($"No module found with ID '{ModuleId}'");
                }
                return new ConsoleResultModel(string.Empty) { Data = lst };
            }

        }

    }
}