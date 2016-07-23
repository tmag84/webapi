using DAW.Models;
using DAW.Utils.Exceptions;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAW.Utils.DB
{
    public class DB_Posts
    {
        //post project
        public static bool PostProject(ProjectModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DB_Config.GetConnectionString();
                    con.Open();

                    ProjectModel proj = DB_AuxGets.GetProjectByName(model.proj_name, con);
                    if (proj != null)
                    {
                        throw new DuplicatedItemException(string.Format("The project {0} already exists.", model.proj_name));
                    }

                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "insert into project(proj_name,proj_creationDate) values(@name,GETUTCDATE())";
                        SqlParameter proj_name = new SqlParameter("@name", System.Data.SqlDbType.VarChar, 30);
                        proj_name.Value = model.proj_name;
                        cmd.Parameters.Add(proj_name);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException e)
            {
                throw new InternalDBException(e.ToString());
            }
            return true;
        }

        //post tag in project
        public static bool PostTagInProject(string name, string tag)
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
                    if (tags.Contains(tag))
                    {
                        throw new DuplicatedItemException(string.Format("Tag {0} already exists in project {1}.", tag, name));
                    }
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "insert into Project_Tag(proj_name,proj_tag) values(@name,@tag)";

                        SqlParameter proj_name = new SqlParameter("@name", System.Data.SqlDbType.VarChar, 30);
                        proj_name.Value = name;
                        cmd.Parameters.Add(proj_name);

                        SqlParameter tag_type = new SqlParameter("@tag", System.Data.SqlDbType.VarChar, 30);
                        tag_type.Value = tag;
                        cmd.Parameters.Add(tag_type);

                        cmd.ExecuteReader();
                    }
                }
                return true;
            }
            catch (SqlException e)
            {
                throw new InternalDBException(e.ToString());
            }
        }

        //post issue in project
        public static bool PostIssue(string name, IssueModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = DB_Config.GetConnectionString();
                    con.Open();

                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "insert into issue(proj_name,issue_title,issue_description,issue_creationDate) values(@name,@title,@description,GETUTCDATE())";
                        SqlParameter proj_name = new SqlParameter("@name", System.Data.SqlDbType.VarChar, 30);
                        proj_name.Value = name;
                        cmd.Parameters.Add(proj_name);

                        SqlParameter iss_title = new SqlParameter("@title", System.Data.SqlDbType.VarChar, 100);
                        iss_title.Value = model.issue_title;
                        cmd.Parameters.Add(iss_title);

                        SqlParameter iss_description = new SqlParameter("@description", System.Data.SqlDbType.VarChar, 500);
                        iss_description.Value = model.issue_description;
                        cmd.Parameters.Add(iss_description);

                        cmd.ExecuteReader();
                    }
                }
                //sucess
                return true;
            }
            catch (SqlException e)
            {
                throw new InternalDBException(e.ToString());
            }
        }

        //post tag in issue
        public static bool PostTagInIssue(string name, int id, string tag)
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
                        throw new NotFoundException(string.Format("The issue {0} was not found.", id));
                    }
                    if (issue.tags.Contains(tag))
                    {
                        throw new DuplicatedItemException(string.Format("The tag {0} already exists in the issue {1}, belonging to project {2}.", tag, id, name));
                    }

                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "insert into issue_tag(proj_name,issue_id,proj_tag) values(@name,@id,@tag)";
                        SqlParameter proj_name = new SqlParameter("@name", System.Data.SqlDbType.VarChar, 30);
                        proj_name.Value = name;
                        cmd.Parameters.Add(proj_name);

                        SqlParameter iss_id = new SqlParameter("@id", System.Data.SqlDbType.Int);
                        iss_id.Value = id;
                        cmd.Parameters.Add(iss_id);

                        SqlParameter tag_type = new SqlParameter("@tag", System.Data.SqlDbType.VarChar, 30);
                        tag_type.Value = tag;
                        cmd.Parameters.Add(tag_type);

                        cmd.ExecuteReader();
                    }
                }
                //sucess
                return true;
            }
            catch (SqlException e)
            {
                throw new InternalDBException(e.ToString());
            }
        }

        //post comment
        public static bool PostComment(string name, int id, CommentModel model)
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
                        throw new NotFoundException("The issue " + id + " does not exist in the project " + name + ".");
                    }
                    if (issue.issue_state == "closed")
                    {
                        throw new ClosedIssueException("The issue " + id + " in the project " + name + " is closed, so it doesn't allow new comments.");
                    }
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "insert into comment(proj_name,issue_id,comment_id,comment_text,comment_creationDate) values(@name,@issue_id,@id,@text,GETUTCDATE())";
                        SqlParameter proj_name = new SqlParameter("@name", System.Data.SqlDbType.VarChar, 100);
                        proj_name.Value = name;
                        cmd.Parameters.Add(proj_name);

                        SqlParameter iss_id = new SqlParameter("@issue_id", System.Data.SqlDbType.Int);
                        iss_id.Value = id;
                        cmd.Parameters.Add(iss_id);

                        SqlParameter cmt_id = new SqlParameter("@id", System.Data.SqlDbType.Int);
                        //because comments can't be removed, the number of comments stored +1 = id for new comment

                        if (issue.comments==null)
                        {
                            cmt_id.Value = 1;
                        }
                        else
                        {
                            cmt_id.Value = issue.comments.Count + 1;
                        }
                       
                        cmd.Parameters.Add(cmt_id);

                        SqlParameter cmt_text = new SqlParameter("@text", System.Data.SqlDbType.VarChar, 200);
                        cmt_text.Value = model.comment_text;
                        cmd.Parameters.Add(cmt_text);

                        cmd.ExecuteReader();
                    }
                }
                //sucess
                return true;
            }
            catch (SqlException e)
            {
                throw new InternalDBException(e.ToString());
            }
        }

    }
}