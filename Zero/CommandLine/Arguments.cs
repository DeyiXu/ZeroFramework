using System;
using System.Collections.Generic;
using System.Text;

namespace Zero.CommandLine
{
    public class Arguments
    {
        private Dictionary<string, string> Parameters = new Dictionary<string, string>();
        public Arguments()
        {

        }
        public Arguments(string[] args)
        {
            Load(args);
        }
        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="args"></param>
        public void Load(string[] args)
        {
            //排除不正确的数据
            List<string> newArgList = new List<string>();
            if (args.Length != 0 && (args.Length % 2) != 0)
            {
                for (int i = 0; i < args.Length - 1; i++)
                {
                    newArgList.Add(args[i]);
                }
            }
            else
            {
                newArgList.AddRange(args);
            }
            //排除参数Key不正确的 Key和Val
            for (int i = 0; i < newArgList.Count; i++)
            {
                if ((i + 1) % 2 != 0 && VerifyArgKey(newArgList[i]))
                {
                    newArgList.RemoveAt(i);
                    newArgList.RemoveAt(i);
                }
            }

            for (int i = 0; i < newArgList.Count; i += 2)
            {
                Add(newArgList[i], newArgList[i + 1]);
            }
        }

        /// <summary>
        /// 验证Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool VerifyArgKey(string key)
        {
            return (key.IndexOf("-") == -1 || key.IndexOf("-") > 0);
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void Add(string key, string val)
        {
            if (Parameters.ContainsKey(key))
            {
                Parameters[key] = val;
            }
            else
            {
                Parameters.Add(key, val);
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            Parameters.Remove(key);
        }
        /// <summary>
        /// 输出指定格式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in Parameters)
            {
                sb.Append($"{item.Key}:{item.Value}" + Environment.NewLine);
            }
            return sb.ToString();
        }

        public string this[string param] => Parameters[param];
    }
}
