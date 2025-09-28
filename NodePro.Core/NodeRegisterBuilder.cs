using NodePro.Abstractions.Interfaces;
using NodePro.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core
{
    public class NodeRegisterBuilder
    {
        public delegate NodeRegister CreateNodeRegisterDelegate(BuilderData data);
        public class BuilderData
        {
            public List<string> Configs = [];

            public Dictionary<string, INodeRegisterTypeHandler> Handlers = [];

            public List<NodeRegisterKey> Keys = [];
        }

        public class BuilderBase(NodeRegisterBuilder.BuilderData data,CreateNodeRegisterDelegate createNodeRegister)
        {
            protected BuilderData Data = data;
            protected CreateNodeRegisterDelegate Create = createNodeRegister;

            public NodeRegister Build()
            {
                return Create(Data);
            }
        }


        public class ConfigRegisterdBuilder(NodeRegisterBuilder.BuilderData data,CreateNodeRegisterDelegate create) : BuilderBase(data,create)
        {
            public ConfigRegisterdBuilder AddConfig(string config)
            {
                Data.Configs.Add(config);
                return this;
            }

            public HandlerRegisteredBuilder Completed()
            {
                return new HandlerRegisteredBuilder(Data,Create);
            }
        }
        public class HandlerRegisteredBuilder(NodeRegisterBuilder.BuilderData data,CreateNodeRegisterDelegate create) : BuilderBase(data,create)
        {
            public HandlerRegisteredBuilder AddHandler<THandler>(string key,THandler? handler=null) where THandler : class, INodeRegisterTypeHandler, new()
            {
                handler ??= Activator.CreateInstance<THandler>();
                Data.Handlers.Add(key,handler);
                return this;
            }

            public RegisterKeyBuilder Completed() { return new RegisterKeyBuilder(Data,Create); }
        }

        public class RegisterKeyBuilder(NodeRegisterBuilder.BuilderData data,CreateNodeRegisterDelegate create) : BuilderBase(data,create)
        {
            public RegisterKeyBuilder AddKey(NodeRegisterKey key)
            {
                Data.Keys.Add(key);
                return this;
            }
        }


        public static ConfigRegisterdBuilder GetBuilder() 
        { 
            BuilderData data = new BuilderData();
            return new ConfigRegisterdBuilder(data,OnBuild); 
        }

        private static NodeRegister OnBuild(BuilderData data)
        {
            try
            {
                NodeRegister register = NodeRegister.Combine(data.Configs.ToArray());
                foreach (var handlerPair in data.Handlers)
                {
                    register.AddHandler(handlerPair.Key, handlerPair.Value);
                }
                foreach (var key in data.Keys)
                {
                    register.AddKey(key);
                }
                register.Scan();
                return register;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Build Error:" + ex.Message);
                return new NodeRegister();
            }

        }

    }
}
