using UnityEngine;
using System.Text.RegularExpressions;

public static class PathUtils
{
    // 主清理方法：删除 "Assets/Resources/" 前缀并处理所有变体
    public static string RemoveResourcesPrefix(string originalPath)
    {
        if (string.IsNullOrEmpty(originalPath)) 
            return originalPath;

        // 步骤1: 统一使用正斜杠（处理混合分隔符情况）
        string normalizedPath = originalPath.Replace('\\', '/');

        // 步骤2: 删除路径开头的特定前缀（不区分大小写）
        string pattern = @"^(assets/resources/|resources/|assets/resources|resources)";
        string result = Regex.Replace(
            normalizedPath, 
            pattern, 
            "", 
            RegexOptions.IgnoreCase
        );

        // 步骤3: 删除残留的引导斜杠
        if (result.StartsWith("/")) 
            result = result.Substring(1);

        // 步骤4: 删除文件扩展名（Resources.Load 不需要）
        int lastDotIndex = result.LastIndexOf('.');
        if (lastDotIndex > 0) 
            result = result.Substring(0, lastDotIndex);

        return result;
    }
}
