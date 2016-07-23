using DAW.Models;
using DAW.Utils.Exceptions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DAW.Utils.DB
{
    public class DB_Gets
    {
        //get all projects
        public static List<ProjectModel> GetAllProjects()
        {
            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DB_Config.GetConnectionString();
                    con.Open();

                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "select * from project";                        
                        SqlDataReader dr = cmd.ExecuteReader();
                        var projects = new List<Dictionary<string, object>>();

                        while (dr.Read())
                        {
                            var proj = new Dictionary<string, object>();
                            for (int i = 0; i < dr.FieldCount; i++)
                            {
                                proj.Add(dr.GetName(i), dr.IsDBNull(i) ? null : dr.GetValue(i));
                            }
                            projects.Add(proj);
                        }
                        string json = JsonConvert.SerializeObject(projects, Formatting.Indented);
                        return (List<ProjectModel>)JsonConvert.DeserializeObject(json, typeof(List<ProjectModel>));
                    }
                }
            }
            catch (SqlException e)
            {
                throw new InternalDBException(e.ToString());
            }
        }
        
        //get project by name with pagination
        public static ProjectModel GetProjectByName(string name, int pageSize, int page)
        {
            ProjectModel project = null;
            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DB_Config.GetConnectionString();
                    con.Open();

                    project = DB_AuxGets.GetProjectByName(name, con);
                    if (project == null)
                    {
                        throw new NotFoundException(string.Format("The project {0} was not found.", name));
                    }

                    project.issues = DB_AuxGets.GetProjectIssues(name, con);

                    if (pageSize * (page-1) > project.issues.Count)
                    {
                        throw new PaginationException(string.Format("The pagination settings are out of bounds for the project {0}.", name));
                    }

                    project.totalIssues = project.issues.Count;
                    project.tags = DB_AuxGets.GetTags(name, 0, con);
                }
            }
            catch (SqlException e)
            {
                throw new InternalDBException(e.ToString());
            }
            return project;
        }

        //get project by name with pagination and issue search by name
        public static ProjectModel SearchProjectIssues(string name, string title, int pageSize, int page)
        {
            ProjectModel proj = null;
            try
            {
                proj = GetProjectByName(name, pageSize, page);
                proj.issues = proj.issues.FindAll(i => i.issue_title.Contains(title));
                proj.totalIssues = proj.issues.Count;

                if (pageSize * (page - 1) > proj.issues.Count)
                {
                    throw new PaginationException(string.Format("The pagination settings are out of bounds for the project {0}.", name));
                }
            }
            catch (MyException e)
            {
                throw e;
            }
            return proj;
        }

        //get project issue by id
        public static IssueModel GetProjectIssueById(string name, int id)
        {
            IssueModel issue = null;
            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DB_Config.GetConnectionString();
                    con.Open();

                    issue = DB_AuxGets.GetIssueById(name, id, con);
                    if (issue == null)
                    {
                        throw new NotFoundException(string.Format("The issue {0}, belonging to project {1}, was not found.", id, name));
                    }

                    issue.tags = DB_AuxGets.GetTags(name, id, con);

                    issue.comments = DB_AuxGets.GetComments(name, id, con);
                }
                return issue;
            }
            catch (SqlException e)
            {
                throw new InternalDBException(e.ToString());
            }
        }

        //get issues_info by proj
        public static List<IssueModel> GetAllProjectIssues(string name)
        {
            List<IssueModel> issues = null;
            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DB_Config.GetConnectionString();
                    con.Open();

                    issues = DB_AuxGets.GetProjectIssues(name, con);
                    if (issues == null)
                    {
                        throw new NotFoundException(string.Format("No issues, belonging to project {1}, were found.", name));
                    }
                }
                return issues;
            }
            catch (SqlException e)
            {
                throw new InternalDBException(e.ToString());
            }
        }

        //get project issue comments
        public static List<CommentModel> GetCommentByIssue(string name, int id)
        {

            List<CommentModel> comments = null;
            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DB_Config.GetConnectionString();
                    con.Open();

                    comments = DB_AuxGets.GetComments(name,id, con);
                    if (comments == null)
                    {
                        throw new NotFoundException(string.Format("The comments, belonging to project {1} issue {2}, were not found.", name,id));
                    }
                }
                return comments;
            }
            catch (SqlException e)
            {
                throw new InternalDBException(e.ToString());
            }           
        }
    }
}