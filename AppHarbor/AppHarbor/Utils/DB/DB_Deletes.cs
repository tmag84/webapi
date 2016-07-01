using DAW.Models;

using DAW.Utils.Exceptions;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAW.Utils.DB
{
    class DB_Deletes
    {
        //remove tag from project (and it's issues as well)
        public static bool DeleteTagFromProject(string name, string tag)
        {
            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DB_Config.GetConnectionString();
                    con.Open();

                    ProjectModel project = DB_AuxGets.GetProjectByName(name, con);
                    if (project == null)
                    {
                        throw new NotFoundException(string.Format("Project {0} was not found.", name));
                    }

                    List<string> tags = DB_AuxGets.GetTags(name, 0, con);
                    if (!tags.Contains(tag))
                    {
                        throw new NotFoundException(string.Format("Tag {0} does not exist in project {1}.", tag, name));
                    }

                    DeleteTagFromProject(name, tag, con);
                }
                return true;
            }
            catch (SqlException e)
            {
                throw new InternalDBException(e.ToString());
            }
        }

        //delete tag from issue
        public static bool DeleteTagFromIssue(string name, int id, string tag)
        {
            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DB_Config.GetConnectionString();
                    con.Open();

                    IssueModel issue = DB_AuxGets.GetIssueById(name, id, con);
                    if (issue == null)
                    {
                        throw new NotFoundException(string.Format("The issue {0}, belonging to project {1}, was not found",id, name));
                    }

                    List<string> tags = DB_AuxGets.GetTags(name, id, con);
                    if (!tags.Contains(tag))
                    {
                        throw new NotFoundException(string.Format("Tag {0} does not exist in project {1}.", tag, name));
                    }

                    DeleteTagFromIssue(name, id, tag, con);
                }
                return true;
            }
            catch (SqlException e)
            {
                throw new InternalDBException(e.ToString());
            }
        }

        //deletes tag from project
        private static bool DeleteTagFromProject(string name, string tag, SqlConnection con)
        {
            //first deletes tag from all issues in the project
            List<IssueModel> issues = DB_AuxGets.GetProjectIssues(name, con);
            foreach (IssueModel issue in issues)
            {
                DeleteTagFromIssue(name, issue.id, tag, con);
            }
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "delete from Project_Tag where name=@name and tag=@tag";

                SqlParameter proj_name = new SqlParameter("@name", System.Data.SqlDbType.VarChar, 30);
                proj_name.Value = name;
                cmd.Parameters.Add(proj_name);

                SqlParameter tag_type = new SqlParameter("@tag", System.Data.SqlDbType.VarChar, 30);
                tag_type.Value = tag;
                cmd.Parameters.Add(tag_type);

                cmd.ExecuteReader();
            }
            return true;
        }

        //deletes tag from issue
        private static bool DeleteTagFromIssue(string name, int id, string tag, SqlConnection con)
        {
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "delete from Issue_Tag where name=@name id=@id and tag=@tag";

                SqlParameter proj_name = new SqlParameter("@name", System.Data.SqlDbType.VarChar, 30);
                proj_name.Value = name;
                cmd.Parameters.Add(proj_name);

                SqlParameter issue_id = new SqlParameter("@id", System.Data.SqlDbType.Int);
                issue_id.Value = id;
                cmd.Parameters.Add(issue_id);

                SqlParameter tag_type = new SqlParameter("@tag", System.Data.SqlDbType.VarChar, 30);
                tag_type.Value = tag;
                cmd.Parameters.Add(tag_type);

                cmd.ExecuteReader();
            }
            return true;
        }
    }
}
