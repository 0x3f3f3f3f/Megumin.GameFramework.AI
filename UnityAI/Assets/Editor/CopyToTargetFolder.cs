using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Megumin;
using System.ComponentModel;
using System;
using UnityEditor.PackageManager;
using System.IO;

public class CopyToTargetFolder : ScriptableObject
{
    public List<string> packageName = new();

    [Path]
    public List<string> targets = new();

    [Path]
    public string SamplesPath = "Assets/Samples/BehaviorTree";
    [Path]
    public string SamplesTargetFolder = "../../Megumin.GameFramework.AI.Samples/AISamples/Assets/Samples/BehaviorTree";

    [Editor]
    public void Copy()
    {
        var infos = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages();
        foreach (var item in infos)
        {
            if (packageName.Contains(item.name))
            {
                Copy(item);
            }
        }

        var samplePath = Path.Combine(MeguminUtility4Unity.ProjectPath, SamplesPath);
        var sampleTarget = Path.Combine(MeguminUtility4Unity.ProjectPath, SamplesTargetFolder);
        CopyFolder(samplePath, sampleTarget, true);
    }

    [Editor]
    public void OpenTarget()
    {
        foreach (var target in targets)
        {
            var root = Path.Combine(MeguminUtility4Unity.ProjectPath, target);
            var targetFolder = Path.GetFullPath(root);
            Debug.Log($"打开 {targetFolder}");
            System.Diagnostics.Process.Start(targetFolder);
        }
    }

    private void Copy(UnityEditor.PackageManager.PackageInfo info)
    {
        Debug.Log(info.ToStringReflection());

        foreach (var target in targets)
        {
            var root = Path.Combine(MeguminUtility4Unity.ProjectPath, target);
            var targetFolder = Path.Combine(root, info.name);
            targetFolder = Path.GetFullPath(targetFolder);
            Debug.Log($"Copy {info.resolvedPath}  To  {targetFolder}");
            if (Directory.Exists(targetFolder))
            {
                Directory.Delete(targetFolder, true);
            }

            CopyFolder(info.resolvedPath, targetFolder);
        }
    }

    /// <summary>
    /// 复制文件夹及文件
    /// </summary>
    /// <param name="sourceFolder">原文件路径</param>
    /// <param name="destFolder">目标文件路径</param>
    /// <returns></returns>
    public int CopyFolder(string sourceFolder, string destFolder, bool overwrite = false)
    {
        try
        {
            //如果目标路径不存在,则创建目标路径
            if (!System.IO.Directory.Exists(destFolder))
            {
                System.IO.Directory.CreateDirectory(destFolder);
            }
            //得到原文件根目录下的所有文件
            string[] files = System.IO.Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = System.IO.Path.GetFileName(file);
                string dest = System.IO.Path.Combine(destFolder, name);
                System.IO.File.Copy(file, dest, overwrite);//复制文件
            }
            //得到原文件根目录下的所有文件夹
            string[] folders = System.IO.Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = System.IO.Path.GetFileName(folder);
                string dest = System.IO.Path.Combine(destFolder, name);
                var dirName = System.IO.Path.GetDirectoryName(folder);
                if (name == ".git")
                {
                    continue;
                }

                CopyFolder(folder, dest, overwrite);//构建目标路径,递归复制文件
            }
            return 1;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return 0;
        }

    }
}
