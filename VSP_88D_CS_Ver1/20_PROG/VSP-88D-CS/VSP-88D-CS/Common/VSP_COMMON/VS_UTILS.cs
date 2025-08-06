using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using VSP_COMMON;
using System.IO;

public static class VS_UTILS
{

    public static string GetDataDir()
    {
        return FolderConstants.DATA;
    }

    public static string GetLogDir()
    {
        return FolderConstants.LOG;
    }

    public static string GetReportDir()
    {
        return FolderConstants.REPORT;
    }

    public static string GetProjectFolder()
    {
        return Path.Combine("D:\\", GetProjectName());
    }

    public static string GetProjectName()
    {
        return Assembly.GetEntryAssembly().GetName().Name;
    }

    public static int GetMaxPmCount()
    {
        string projName = GetProjectName();
        if (projName.Contains("PRO1_PLUS")|| projName.Contains("PRO1_H")|| projName.Contains("PRO1_01"))
            return (int)eLayerType.SINGLE;
        else //if (ContainsStr(GetProjectName(), L"PRO_PLUS") )
            return (int)eLayerType.DUAL;
    }
}
