using GraphHop.Shared;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;
using Rhino.PlugIns;

namespace GraphHop.PluginRhino
{
    ///<summary>
    /// Every RhinoCommon .rhp assembly must have one and only one PlugIn-derived
    /// class. DO NOT create instances of this class yourself. It is the
    /// responsibility of Rhino to create an instance of this class.
    /// To complete plug-in information, please also see all PlugInDescription
    /// attributes in AssemblyInfo.cs
    ///</summary>
    public class PluginRhino : Rhino.PlugIns.PlugIn
    {
        private PlugInLoadTime _loadTime;

        public PluginRhino()
        {
            Instance = this;
        }

        
        public static IGremlinGeneric Gremlin;
        
        protected override LoadReturnCode OnLoad(ref string errorMessage)
        {
            var endpoint = "127.0.0.1";
            // This uses the default Neptune and Gremlin port, 8182
            var gremlinServer = new GremlinServer(endpoint, 8182, enableSsl: false);
            var gremlinClient = new GremlinClient(gremlinServer);
            var remoteConnection = new DriverRemoteConnection(gremlinClient, "g");
            var gremlin = AnonymousTraversalSource.Traversal().WithRemote(remoteConnection);
            Gremlin = new GremlinGeneric(gremlin);
            
            return base.OnLoad(ref errorMessage);
        }

        ///<summary>Gets the only instance of the MyRhinoPlugin1 plug-in.</summary>
        public static PluginRhino Instance { get; private set; }

        // You can override methods here to change the plug-in behavior on
        // loading and shut down, add options pages to the Rhino _Option command
        // and maintain plug-in wide options in a document.
    }
}