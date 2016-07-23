using DAW.Models;
using DAW.Utils.Exceptions;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAW.Utils.DB
{
    class DB_Puts
    {
        //put comment
        public static bool PutComment(string name, int id, CommentModel model)
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
                        throw new NotFoundException(string.Format("The issue {0} was not found in the project {1}.", id, name));
                    }

                    if (DB_AuxGets.GetIssueById(name, id, con).issue_state == "closed")
                    {
                        throw new ClosedIssueException(string.Format("The issue {0} is closed, so it does not allow edit to comments.", id));
                    }

                    //gets existing comment to edit
                    CommentModel old_model = DB_AuxGets.GetComments(name, id, con).Find(c => c.comment_id == model.comment_id);
                    if (old_model == null)
                    {
                        throw new NotFoundException("The comment with id " + model.comment_id + " does not exist in this issue.");
                    }

                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "update comment set comment_text=@text and modifiedDate=GETUTCDATE() where proj_name=@name and issue_id=@issue_id and comment_id=@id";
                        SqlParameter proj_name = new SqlParameter("@proj_name", System.Data.SqlDbType.VarChar, 30);
                        proj_name.Value = name;
                        cmd.Parameters.Add(proj_name);

                        SqlParameter iss_id = new SqlParameter("@issue_id", System.Data.SqlDbType.Int);
                        iss_id.Value = id;
                        cmd.Parameters.Add(iss_id);

                        SqlParameter cmt_id = new SqlParameter("@id", System.Data.SqlDbType.Int);
                        //because comments can't be removed, the number of comments stored +1 = id for new comment
                        cmt_id.Value = issue.comments.Count + 1;
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

        //put new state in issue
        public static bool PutStateInIssue(string name, int id, string new_state)
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
                        throw new NotFoundException(string.Format("The issue {0}, belonging to project,{1} was not found.", name, id));
                    }

                    List<string> states = DB.DB_AuxGets.GetStates(con);
                    if (!states.Contains(new_state))
                    {
                        throw new InvalidStateException(string.Format("The state {0} is an invalid state to use.", new_state));
                    }

                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "update issue set issue_state=@state where proj_name=@name and issue_id=@id";
                        SqlParameter proj_name = new SqlParameter("@proj_name", System.Data.SqlDbType.VarChar, 30);
                        proj_name.Value = name;
                        cmd.Parameters.Add(proj_name);

                        SqlParameter iss_id = new SqlParameter("@id", System.Data.SqlDbType.Int);
                        iss_id.Value = id;
                        cmd.Parameters.Add(iss_id);

                        SqlParameter state_type = new SqlParameter("@state", System.Data.SqlDbType.VarChar, 30);
                        state_type.Value = new_state;
                        cmd.Parameters.Add(state_type);

                        cmd.ExecuteReader();
                    }
                }
            }
            catch (SqlException e)
            {
                throw new InternalDBException(e.ToString());
            }
            return true;
        }
    }
}