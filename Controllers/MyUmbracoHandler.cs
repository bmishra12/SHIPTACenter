using Examine;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Web.Mvc;
using umbraco.cms.businesslogic.web;
using Umbraco.Core;

using umbraco.BusinessLogic;
using UmbracoExamine;


namespace UmbracoShipTac.Controllers
{
    public class MyUmbracoHandler : ApplicationEventHandler
    {



            protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
            {
                ExamineManager.Instance.IndexProviderCollection["WebsiteIndexer"].GatheringNodeData += WebsiteIndexer_OnGatheringNodeData;
                ExamineManager.Instance.IndexProviderCollection["ResourceIndexer"].GatheringNodeData += ResourceIndexer_OnGatheringNodeData;

            }
            private void WebsiteIndexer_OnGatheringNodeData(object sender, IndexingNodeDataEventArgs e)
            {

                //if (e.Fields.ContainsKey("relatedBlogArticles"))
                //{
                //    e.Fields["_relatedBlogArticles"] = e.Fields["relatedBlogArticles"].Replace(',', ' ');
                //}

                InjectGroups(e);
            }

            private void ResourceIndexer_OnGatheringNodeData(object sender, IndexingNodeDataEventArgs e)
            {

                InjectGroupsResource(e);
            }


            /// <summary>
            /// munge into one field
            /// </summary>
            /// <param name="e"></param>
            private void InjectGroups(IndexingNodeDataEventArgs e)
            {
                var node = new umbraco.NodeFactory.Node(e.NodeId);
               // Umbraco.Core.Logging.LogHelper.Info(this.GetType(), string.Format("test {0}", e.NodeId));

  

                try
                {
                    //umbraco.cms.businesslogic.web.Access.IsProtected(int.Parse(e.Node.Attribute("id").Value), e.Node.Attribute("path").Value);
                    if (umbraco.cms.businesslogic.web.Access.IsProtected(e.NodeId, node.Path))
                    {
                        var groups = Access.GetAccessingMembershipRoles(node.Id, node.Path);

                      //  Umbraco.Core.Logging.LogHelper.Info(this.GetType(), string.Format("test in the succes if"));

                        e.Fields.Add("GroupAccess", String.Join(",", groups));
                        e.Fields.Add("IsPublic", "false");
                    }

                    else
                    {
                    //    Umbraco.Core.Logging.LogHelper.Info(this.GetType(), string.Format("test in the succes else"));


                        e.Fields.Add("GroupAccess", "0");
                        e.Fields.Add("IsPublic", "true");

                    }
                }
                catch (Exception ex)
                {
                    Umbraco.Core.Logging.LogHelper.Info(this.GetType(), string.Format("test {0}", ex.ToString()));
                }
            }

            private void InjectGroupsResource(IndexingNodeDataEventArgs e)
            {
                var node = new umbraco.NodeFactory.Node(e.NodeId);
                // Umbraco.Core.Logging.LogHelper.Info(this.GetType(), string.Format("test {0}", e.NodeId));



                try
                {
                    //umbraco.cms.businesslogic.web.Access.IsProtected(int.Parse(e.Node.Attribute("id").Value), e.Node.Attribute("path").Value);
                    if (umbraco.cms.businesslogic.web.Access.IsProtected(e.NodeId, node.Path))
                    {
                        var groups = Access.GetAccessingMembershipRoles(node.Id, node.Path);

                        //  Umbraco.Core.Logging.LogHelper.Info(this.GetType(), string.Format("test in the succes if"));

                        e.Fields.Add("GroupAccess", String.Join(",", groups));
                        e.Fields.Add("IsPublic", "false");
                    }

                    else
                    {
                        //    Umbraco.Core.Logging.LogHelper.Info(this.GetType(), string.Format("test in the succes else"));
                        //Resource will not have any public pages
                        //this condition will hit if the move node is not successfully inherits the restriction roles..

                        e.Fields.Add("GroupAccess", "NOTPROTECTED");
                        e.Fields.Add("IsPublic", "false");

                    }
                }
                catch (Exception ex)
                {
                    Umbraco.Core.Logging.LogHelper.Info(this.GetType(), string.Format("test {0}", ex.ToString()));
                }
            }





    }
}