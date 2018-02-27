using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SelfbotV2
{
    class TagsService
    {
        private List<TagInfo> tags = new List<TagInfo>();
        public async Task AddModulesAsync(Assembly assembly)
        {
            foreach (var module in assembly.GetModules()
                .SelectMany(assm => assm.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(ITagModule))))
            {
                var q = module.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                foreach (var propertyInfo in q)
                {
                    var input = Expression.Parameter(typeof(object), "input");
                    var tagName = ((TagAttribute)propertyInfo.GetCustomAttributes(typeof(TagAttribute), false).First()).v;
                    //tags.Add(new TagInfo(tagName,new TagInfo.action(Delegate.CreateDelegate(module,typeof(string),propertyInfo))));
                }
            }
        }
    }

    internal class TagInfo
    {
        public string Name { get; }
        public delegate string action(string s);

        public action myAction;
        public TagInfo(string Name, action action)
        {
            this.Name = Name;
            myAction = new action(action);
        }
    }

    internal interface ITagModule
    {

    }

    class Asd : ITagModule
    {
        [Tag("asd")]
        public string Dsa(string asd)
        {
            return "";
        }
    }

    internal class TagAttribute : Attribute
    {
        public string v;

        public TagAttribute(string v)
        {
            this.v = v;
        }
    }
}
