using System.Collections.Generic;
using System.Data.SqlClient;
using Newtonsoft.Json;
using DAW.Models;


namespace DAW.Utils.DB
{
    class DB_AuxGets
    {
        //get project by name
        public static ProjectModel GetProjectByName(string name, SqlConnection con)
        {
            ProjectModel project = null;
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select * from project where name=@name";
                SqlParameter proj_name = new SqlParameter("@name", System.Data.SqlDbType.VarChar, 30);
                proj_name.Value = name;
                cmd.Parameters.Add(proj_name);

                SqlDataReader dr = cmd.ExecuteReader();
                if (!dr.HasRows)
                {
                    //if there aren't any rows, then project does not exist
                    return null;
                }

                while (dr.Read())
                {
                    var data = new Dictionary<string, object>();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        data.Add(dr.GetName(i), dr.IsDBNull(i) ? null : dr.GetValue(i));
                    }
                    string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                    project = (ProjectModel)JsonConvert.DeserializeObject(json, typeof(ProjectModel));
                }
                dr.Close();
            }
            return project;
        }

        //get all project's issues
        public static List<IssueModel> GetProjectIssues(string name, SqlConnection con)
        {
            List<IssueModel> issues = new List<IssueModel>();
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select * from issue where name=@name";
                SqlParameter proj_name = new SqlParameter("@name", System.Data.SqlDbType.VarChar, 30);
                proj_name.Value = name;
                cmd.Parameters.Add(proj_name);

                //con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var data = new Dictionary<string, object>();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        data.Add(dr.GetName(i), dr.IsDBNull(i) ? null : dr.GetValue(i));
                    }
                    string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                    issues.Add((IssueModel)JsonConvert.DeserializeObject(json, typeof(IssueModel)));
                }
                dr.Close();
            }
            return issues;
        }

        //get issue by id
        public static IssueModel GetIssueById(string name, int id, SqlConnection con)
        {
            IssueModel issue = null;
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select * from Issue where name=@name and id=@id";
                SqlParameter proj_name = new SqlParameter("@name", System.Data.SqlDbType.VarChar, 30);
                proj_name.Value = name;
                cmd.Parameters.Add(proj_name);

                SqlParameter iss_id = new SqlParameter("@id", System.Data.SqlDbType.Int);
                iss_id.Value = id;
                cmd.Parameters.Add(iss_id);

                SqlDataReader dr = cmd.ExecuteReader();
                if (!dr.HasRows)
                {
                    //if issue does not exist, return null
                    return null;
                }

                while (dr.Read())
                {
                    var data = new Dictionary<string, object>();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        data.Add(dr.GetName(i), dr.IsDBNull(i) ? null : dr.GetValue(i));
                    }
                    string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                    issue = (IssueModel)JsonConvert.DeserializeObject(json, typeof(IssueModel));
                }

                dr.Close();
            }

            return issue;
        }

        //get tags by project name and issue id (optional)
        public static List<string> GetTags(string name, int id, SqlConnection con)
        {
            List<string> tags = new List<string>();
            //command to get all the project issues
            using (SqlCommand cmd = con.CreateCommand())
            {
                //if id==0, then we want all the project tags
                if (id == 0)
                {
                    cmd.CommandText = "select * from Project_Tag where name=@name";
                }
                //otherwise, we only want the tags for the issue
                else
                {
                    cmd.CommandText = "select * from Issue_Tag where name=@name and id=@id";
                    SqlParameter iss_id = new SqlParameter("@id", System.Data.SqlDbType.Int);
                    iss_id.Value = id;
                    cmd.Parameters.Add(iss_id);
                }

                SqlParameter proj_name = new SqlParameter("@name", System.Data.SqlDbType.VarChar, 30);
                proj_name.Value = name;
                cmd.Parameters.Add(proj_name);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    tags.Add(dr["tag"].ToString());
                }
                dr.Close();
            }
            return tags;
        }

        //gets comments for issue
        public static List<CommentModel> GetComments(string name, int id, SqlConnection con)
        {
            List<CommentModel> comments = new List<CommentModel>();
            //command to get all the project issue comments
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select id,text,creationDate from Comment where name=@name and issue_id=@id order by creationDate desc";

                SqlParameter proj_name = new SqlParameter("@name", System.Data.SqlDbType.VarChar, 30);
                proj_name.Value = name;
                cmd.Parameters.Add(proj_name);

                SqlParameter iss_id = new SqlParameter("@id", System.Data.SqlDbType.Int);
                iss_id.Value = id;
                cmd.Parameters.Add(iss_id);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var data = new Dictionary<string, object>();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        data.Add(dr.GetName(i), dr.IsDBNull(i) ? null : dr.GetValue(i));
                    }
                    string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                    comments.Add((CommentModel)JsonConvert.DeserializeObject(json, typeof(CommentModel)));
                }
                dr.Close();
            }
            return comments;
        }

        //get accepted states
        public static List<string> GetStates(SqlConnection con)
        {
            List<string> states = new List<string>();
            //command to get all the project issues
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select state from State";
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    states.Add(dr["state"].ToString());
                }
            }
            return states;
        }
    }
}
