﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace TSManager.Properties {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TSManager.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性，对
        ///   使用此强类型资源类的所有资源查找执行重写。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///&lt;SyntaxDefinition name=&quot;Json&quot; extensions=&quot;.js&quot; xmlns=&quot;http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008&quot;&gt;
        ///  &lt;Color name=&quot;Digits&quot; foreground=&quot;#8700FF&quot; exampleText=&quot;3.14&quot; /&gt;
        ///  &lt;Color name=&quot;Value&quot; foreground=&quot;#000CFF&quot; exampleText=&quot;var text = &amp;quot;Hello, World!&amp;quot;;&quot; /&gt;
        ///  &lt;Color name=&quot;ParamName&quot; foreground=&quot;#057500&quot;  exampleText=&quot;var text = &amp;quot;Hello, World!&amp;quot;;&quot; /&gt;
        ///  &lt;RuleSet ignoreCase=&quot;false&quot;&gt;
        ///    &lt;Keywords color=&quot;Digits&quot; &gt;
        ///      &lt;Word&gt;true&lt;/Word [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string json_xshd {
            get {
                return ResourceManager.GetString("json.xshd", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 {
        ///  &quot;prefix_0&quot;: &quot;无&quot;,&quot;prefix_1&quot;: &quot;大&quot;,
        ///  &quot;prefix_2&quot;: &quot;巨大&quot;,
        ///  &quot;prefix_3&quot;: &quot;危险&quot;,
        ///  &quot;prefix_4&quot;: &quot;凶残&quot;,
        ///  &quot;prefix_5&quot;: &quot;锋利&quot;,
        ///  &quot;prefix_6&quot;: &quot;尖锐&quot;,
        ///  &quot;prefix_7&quot;: &quot;微小&quot;,
        ///  &quot;prefix_8&quot;: &quot;可怕&quot;,
        ///  &quot;prefix_9&quot;: &quot;小&quot;,
        ///  &quot;prefix_10&quot;: &quot;钝&quot;,
        ///  &quot;prefix_11&quot;: &quot;倒霉&quot;,
        ///  &quot;prefix_12&quot;: &quot;笨重&quot;,
        ///  &quot;prefix_13&quot;: &quot;可耻&quot;,
        ///  &quot;prefix_14&quot;: &quot;重&quot;,
        ///  &quot;prefix_15&quot;: &quot;轻&quot;,
        ///  &quot;prefix_16&quot;: &quot;精准&quot;,
        ///  &quot;prefix_17&quot;: &quot;迅速&quot;,
        ///  &quot;prefix_18&quot;: &quot;急速&quot;,
        ///  &quot;prefix_19&quot;: &quot;恐怖&quot;,
        ///  &quot;prefix_20&quot;: &quot;致命&quot;,
        ///  &quot;prefix_21&quot;: &quot;可靠&quot;,
        ///  &quot;prefix_22&quot;: &quot;可畏&quot;,
        ///  &quot;prefix_23&quot;: &quot;无力&quot;,
        ///  &quot;prefix_24&quot;: &quot;粗笨&quot;,
        ///  &quot;p [字符... 的本地化字符串。
        /// </summary>
        internal static string Prefix {
            get {
                return ResourceManager.GetString("Prefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找 System.Byte[] 类型的本地化资源。
        /// </summary>
        internal static byte[] Texture {
            get {
                object obj = ResourceManager.GetObject("Texture", resourceCulture);
                return ((byte[])(obj));
            }
        }
    }
}
