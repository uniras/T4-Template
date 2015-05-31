#region T4ディレクティブ設定
/*
<#@ template debug="false" hostspecific="true" language="C#" #>
<#@ import namespace="System.IO" #>
<#@ import namespace="System.Collections.Generic" #>
<#@ import namespace="System.Text.RegularExpressions" #>
<#@ import namespace="System.Windows.Forms" #>
<#@ import namespace="System.Linq" #>
<#@ import namespace="System.Drawing" #>
<#@ output extension="output.txt" #>
プロジェクト内のクラスを利用する場合はコメントを外す
<#//@ assembly name="$(ProjectDir)$(OutDir)$(TargetFileName)" #>
<#@ assembly name="System.Core" #>
<#@ assembly name="System.Drawing" #>
<#@ assembly name="System.Windows.Forms" #>
<# GenerationEnvironment.Length = 0; $safeitemname$_Start(); #>
<#//*///#><#+/*
#endregion
#region C#クラス設定
//C#上でMainメソッドを有効にする場合はコメントを外す
//#define USEMAIN
//T4関連クラスのインテリセンスを有効にしたい場合はコメントを外す(要アセンブリ設定)
#define USET4CLASS
#endregion
#region クラス定義
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Drawing;
//WindowsForms関連クラスを使うときはコメントを外す
//using System.Windows.Forms;

class $safeitemname$
#if USET4CLASS
    : Microsoft.VisualStudio.TextTemplating.TextTransformation
#endif
{
#if USET4CLASS
#pragma warning disable 0169, 0649
    Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost Host;
#pragma warning restore 0169, 0649
#endif
#endregion
    //*///#><#+

    //ここにC#コードを記述します、$safeitemname$_Startメソッドが最初に呼ばれます
    public void $safeitemname$_Start()
    {
        Write("Hello World!!");
    }

    //*///#><#+/*
#region Mainメソッド
    public static void Main(string[] args)
    {
        $safeitemname$ s = new $safeitemname$();
        s.$safeitemname$_Start();
        s.FinalWrite();        
    }
#endregion
#region ベースメソッド
#if USET4CLASS
    public override string TransformText()
    {
        return "";
    }

    public void FinalWrite()
    {
        System.Console.Write(GenerationEnvironment);
    }
#else
    public void Write(string val)
    {
        System.Console.Write(val);
    }

    public void WriteLine(string val)
    {
        System.Console.WriteLine(val);
    }

    public void Write(string val, params object[] arg)
    {
        System.Console.Write(val, arg);
    }

    public void WriteLine(string val, params object[] arg)
    {
        System.Console.WriteLine(val, arg);
    }

    public void FinalWrite()
    {
    }
#endif
}
#endregion
//*///#>