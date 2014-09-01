using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;

namespace BCC.Core.Task
{

    public class BCCTaskProject
    {
        #region BCCTaskProject
        private Guid _projectID = Guid.Empty;
        private string _projectName = string.Empty;
        private int _priority = 0;
        private string _envName = string.Empty;
        private int _visible = 1; // 1 - visible

        public Guid ProjectID
        {
            get
            {
                return _projectID;
            }

            set
            {
                _projectID = value;
            }
        }

        public string ProjectName
        {
            get
            {
                return _projectName;
            }

            set
            {
                _projectName = value;
            }
        }

        public int ProjectPriority
        {
            get
            {
                return _priority;
            }

            set
            {
                _priority = value;
            }
        }

        public string ProjectEnvironment
        {
            get
            {
                return _envName;
            }

            set
            {
                _envName = value;
            }
        }

        public int ProjectVisible
        {
            get
            {
                return _visible;
            }

            set
            {
                _visible = value;
            }
        }
        #endregion 

        /// <summary>
        /// Add Task Project 
        /// </summary>
        /// <param name="taskProject"></param>
        /// <returns></returns>
        public static DataTable AddTaskProject(BCCTaskProject taskProject)
        {
            DataTable dt = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskList_CreateProject]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@projectName", taskProject.ProjectName.Trim());
                command.Parameters.Add(param);

                param = new SqlParameter("@envName", taskProject.ProjectEnvironment);
                command.Parameters.Add(param);

                param = new SqlParameter("@projectPriority", taskProject.ProjectPriority);
                command.Parameters.Add(param);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dt = new DataTable();
                dt.Load(reader);
            }

            return dt;
        }

        /// <summary>
        /// Update Task Project 
        /// </summary>
        /// <param name="taskProject"></param>
        /// <returns>Data table</returns>
        public static DataTable UpdateTaskProject(BCCTaskProject taskProject)
        {
            DataTable dt = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskList_UpdateProject]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@projectID", taskProject.ProjectID);
                command.Parameters.Add(param);

                param = new SqlParameter("@projectName", taskProject.ProjectName.Trim());
                command.Parameters.Add(param);

                param = new SqlParameter("@envName", taskProject.ProjectEnvironment);
                command.Parameters.Add(param);

                param = new SqlParameter("@projectPriority", taskProject.ProjectPriority);
                command.Parameters.Add(param);

                param = new SqlParameter("@showFlag", taskProject.ProjectVisible);
                command.Parameters.Add(param);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dt = new DataTable();
                dt.Load(reader);
            }

            return dt;
        }

        /// <summary>
        /// Retrieve Projects
        /// </summary>
        /// <param name="taskProject"></param>
        /// <returns>Data table</returns>
        public static DataTable RetrieveProjects(BCCTaskProject taskProject)
        {
            DataTable dt = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskList_Projects]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@showFlag", taskProject.ProjectVisible);
                command.Parameters.Add(param);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dt = new DataTable();
                dt.Load(reader);
            }

            return dt;
        }

        public static DataTable ProjectActivation(BCCTaskProject taskProject)
        {
            DataTable dt = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskList_ActivationProject]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@projectID", taskProject.ProjectID);
                command.Parameters.Add(param);

                param = new SqlParameter("@showFlag", taskProject.ProjectVisible);
                command.Parameters.Add(param);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dt = new DataTable();
                dt.Load(reader);
            }

            return dt;
        }

    }

    public class BCCTaskEnvironment
    {
        #region BCCTaskEnvironment
        private Guid _envID = Guid.Empty;
        private string _envName = string.Empty;
        private string _envDesc = string.Empty;
        private string _machineName = string.Empty;
        private string _databaseName = string.Empty;
        private int _display = 1; // 1 - visible

        /// <summary>
        /// Environment ID
        /// </summary>
        public Guid EnvID
        {
            get
            {
                return _envID;
            }

            set
            {
                _envID = value;
            }
        }

        public string EnvName
        {
            get
            {
                return _envName;
            }

            set
            {
                _envName = value;
            }
        }

        public string EnvDesc
        {
            get
            {
                return _envDesc;
            }

            set
            {
                _envDesc = value;
            }
        }

        public string MachineName
        {
            get
            {
                return _machineName;
            }

            set
            {
                _machineName = value;
            }
        }

        public string DatabaseName
        {
            get
            {
                return _databaseName;
            }

            set
            {
                _databaseName = value;
            }
        }

        public int EnvVisible
        {
            get
            {
                return _display;
            }

            set
            {
                _display = value;
            }
        }
        #endregion

        /// <summary>
        /// Add Task Environment
        /// </summary>
        /// <param name="taskEnv"></param>
        /// <returns></returns>
        public static DataTable AddTaskEnvironment(BCCTaskEnvironment taskEnv)
        {
            DataTable dt = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskList_CreateEnv]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@envName", taskEnv.EnvName.Trim());
                command.Parameters.Add(param);

                param = new SqlParameter("@envDescription", taskEnv.EnvDesc.Trim());
                command.Parameters.Add(param);

                param = new SqlParameter("@machineName", taskEnv.MachineName.Trim());
                command.Parameters.Add(param);

                param = new SqlParameter("@databaseName", taskEnv.DatabaseName.Trim());
                command.Parameters.Add(param);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dt = new DataTable();
                dt.Load(reader);
            }

            return dt;
        }

        /// <summary>
        /// Update Task Environment
        /// </summary>
        /// <param name="taskProject"></param>
        /// <returns>Data table</returns>
        public static DataTable UpdateTaskEnvironment(BCCTaskEnvironment taskEnv)
        {
            DataTable dt = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskList_UpdateEnv]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@envID", taskEnv.EnvID);
                command.Parameters.Add(param);

                param = new SqlParameter("@envName", taskEnv.EnvName.Trim());
                command.Parameters.Add(param);

                param = new SqlParameter("@envDescription", taskEnv.EnvDesc.Trim());
                command.Parameters.Add(param);

                param = new SqlParameter("@machineName", taskEnv.MachineName.Trim());
                command.Parameters.Add(param);

                param = new SqlParameter("@databaseName", taskEnv.DatabaseName.Trim());
                command.Parameters.Add(param);

                param = new SqlParameter("@showFlag", taskEnv.EnvVisible);
                command.Parameters.Add(param);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dt = new DataTable();
                dt.Load(reader);
            }

            return dt;
        }

        /// <summary>
        /// Retrieve Task Environment
        /// </summary>
        /// <param name="taskProject"></param>
        /// <returns>Data table</returns>
        public static DataTable RetrieveEnvironments(BCCTaskEnvironment taskEnv)
        {
            DataTable dt = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskList_Environments]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@showFlag", taskEnv.EnvVisible);
                command.Parameters.Add(param);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dt = new DataTable();
                dt.Load(reader);
            }

            return dt;
        }

        /// <summary>
        /// Retrieve Task Environment
        /// </summary>
        /// <param name="taskProject"></param>
        /// <returns>Data table</returns>
        public static DataTable EnvironmentActivation(BCCTaskEnvironment taskEnv)
        {
            DataTable dt = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskList_ActivationEnv]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@envID", taskEnv.EnvID);
                command.Parameters.Add(param);

                param = new SqlParameter("@showFlag", taskEnv.EnvVisible);
                command.Parameters.Add(param);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dt = new DataTable();
                dt.Load(reader);
            }

            return dt;
        }
    }
    
    public class BCCTask
    {
        #region BCCTask Properties
        private Guid _taskId = Guid.Empty;
        private string _taskRef = string.Empty;
        private string _taskTitle = string.Empty;
        private string _taskDescription = string.Empty;
        private int _taskSpeedCode = 101; // Default unassigned speedcode
        private int _taskPriority = 1;  // 1 is highest priority.
        private string _createdbyUser = string.Empty;
        private string _updatebyUser = string.Empty;
        private DateTime _taskDueDate = System.DateTime.UtcNow.AddDays(30);
        private DateTime _taskCreatedDate = System.DateTime.UtcNow;
        private string _taskStatus = Enum.GetName(typeof(TaskState), TaskState.Created);
        private string _taskType = string.Empty;
        private double _taskDuration = 0.0; // Default task duration is 0 hrs.
        private string _taskProjectName = string.Empty;
        private int _visible = 1;
        private string _taskAssignedToUserName = string.Empty;
               
        public Guid TaskID
        {
            get
            {
                return _taskId;
            }
            set
            {
                _taskId = value;
            }
        }

        public string TaskRef
        {
            get
            {
                return _taskRef;
            }
            set
            {
                _taskRef = value;
            }
        }

        public string TaskTitle
        {
            get
            {
                return _taskTitle;
            }
            set
            {
                _taskTitle = value;
            }
        }

        public string TaskDescription
        {
            get
            {
                return _taskDescription;
            }
            set
            {
                _taskDescription = value;
            }
        }

        public double TaskDuration
        {
            get
            {
                return _taskDuration;
            }
            set
            {
                _taskDuration = value;
            }
        }

        public int TaskSpeedCode
        {
            get
            {
                return _taskSpeedCode;
            }
            set
            {
                _taskSpeedCode = value;
            }
        }

        public int TaskPriority
        {
            get
            {
                return _taskPriority;
            }
            set
            {
                _taskPriority = value;
            }
        }

        public string CreatedbyUser
        {
            get
            {
                return _createdbyUser;
            }
            set
            {
                _createdbyUser = value;
            }
        }

        public string UpdatedbyUser
        {
            get
            {
                return _updatebyUser;
            }
            set
            {
                _updatebyUser = value;
            }
        }

        public DateTime TaskDueDate
        {
            get
            {
                return _taskDueDate;
            }

            set
            {
                _taskDueDate = value; 
            }
        }

        public DateTime TaskCreatedDate
        {
            get
            {
                return _taskCreatedDate;
            }

            set
            {
                _taskCreatedDate = value;
            }
        }

        public string TaskStatus
        {
            get
            {
                return _taskStatus;
            }
            set
            {
                _taskStatus = value;
            }
        }

        public string TaskType
        {
            get
            {
                return _taskType;
            }
            set
            {
                _taskType = value;
            }
        }

        public string TaskProjectName
        {
            get
            {
                return _taskProjectName;
            }
            set
            {
                _taskProjectName = value;
            }
        }

        public int TaskVisible
        {
            get
            {
                return _visible;
            }

            set
            {
                _visible = value;
            }
        }

        public string AssignedToUser
        {
            get
            {
                return _taskAssignedToUserName;
            }

            set
            {
                _taskAssignedToUserName = value;
            }
        }
        
        #endregion

        public enum TaskState
        {
            Pending = 0,
            Deferred = 1,
            InProgress = 2,
            Completed = 3,
            InReview = 4,
            Reviewed = 5,
            Created = 6,
            Remark = 7
        }

        public enum TaskTypeState
        {
            Once,
            Daily,
            Weekly,
            Monthly
        }
        
        public BCCTask()
        {
        }

        public static BCCTaskSystemMessage VerifyTask(BCCTask task)
        {
            BCCTaskSystemMessage systemMessage = null;
            string errorMessage = string.Empty;

            if (task.TaskStatus == BCCTask.TaskState.Completed.ToString() && task.AssignedToUser == string.Empty)
            {
                errorMessage = string.Format("Un-assigned task #{0} cannot be set to Completed", task.TaskRef);
                systemMessage = new BCCTaskSystemMessage("101", errorMessage);
            }
            else
                if (task.TaskStatus == BCCTask.TaskState.Completed.ToString()
                    && task.AssignedToUser != string.Empty
                    && task.TaskDuration == 0)
                {
                    errorMessage = string.Format("The number of hours spent for the task #{0} must be specified before setting it to Completed", task.TaskRef);
                    systemMessage = new BCCTaskSystemMessage("100", errorMessage);
                }

            return systemMessage;
        }

        /// <summary>
        /// Create a Task
        /// </summary>
        /// <param name="task">BCC Task</param>
        /// <returns>DataTable</returns>
        public static void CloneTask()
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskList_CloneTask]", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                command.ExecuteNonQuery();

            }
        }

        /// <summary>
        /// Create a Task
        /// </summary>
        /// <param name="task">BCC Task</param>
        /// <returns>DataTable</returns>
        public static DataTable CreateTask(BCCTask task)
        {
            DataTable dt = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskList_CreateTask]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("taskTitle", task.TaskTitle);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskDescription", task.TaskDescription);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskSpeedCode", task.TaskSpeedCode);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskPriority", task.TaskPriority);
                command.Parameters.Add(param);

                param = new SqlParameter("@createdbyUser", task.CreatedbyUser);
                command.Parameters.Add(param);

                //@assignedToUser
                param = new SqlParameter("@assignedToUser", task.AssignedToUser);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskDueDate", task.TaskDueDate);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskType", task.TaskType);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskDuration", task.TaskDuration);
                command.Parameters.Add(param);

                param = new SqlParameter("@projectName", task.TaskProjectName);
                command.Parameters.Add(param);

                //@taskId
                param = new SqlParameter("@taskID", System.Guid.Empty);
                param.Direction = ParameterDirection.Output;
                command.Parameters.Add(param);
                
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dt = new DataTable();
                dt.Load(reader);

                reader.Close();
            }

            return dt;
        }

        /// <summary>
        /// Create a Task
        /// </summary>
        /// <param name="task">BCC Task</param>
        ///// 		@taskID uniqueIdentifier,
        //@taskTitle    nvarchar(50),
        //@taskDescription nvarchar(max),
        //@taskSpeedcode int,
        //@taskPriority int,
        //@updatedbyUser nvarchar(50),
        //@taskDueDate datetime,
        //@taskType nvarchar(50),
        //@taskDuration int,
        //@taskStatus nvarchar(50),
        //@projectName nvarchar(50),
        //@showFlag bit
        /// <returns>DataTable</returns>
        public static void UpdateTask(BCCTask task)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskList_UpdateTask]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@taskID", task.TaskID);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskTitle", task.TaskTitle);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskDescription", task.TaskDescription);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskSpeedCode", task.TaskSpeedCode);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskPriority", task.TaskPriority);
                command.Parameters.Add(param);

                param = new SqlParameter("@updatedbyUser", task.UpdatedbyUser);
                command.Parameters.Add(param);

                //@assignedToUser
                param = new SqlParameter("@assignedToUser", task.AssignedToUser);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskDueDate", task.TaskDueDate);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskType", task.TaskType);
                command.Parameters.Add(param);

                //param = new SqlParameter("@taskStatus", task.TaskStatus);
                //command.Parameters.Add(param);

                param = new SqlParameter("@projectName", task.TaskProjectName);
                command.Parameters.Add(param);

                param = new SqlParameter("@showFlag", task.TaskVisible);
                command.Parameters.Add(param);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Retrieve All Tasks whatever the case may be.
        /// </summary>
        /// <param name="task">BCC Task</param>
        /// <returns>DataTable</returns>
        public static DataTable RetrieveAllTasks(BCCTask task)
        {
            DataTable dt = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskList_AllTasks]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@showFlag", task.TaskVisible);
                command.Parameters.Add(param);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dt = new DataTable();
                dt.Load(reader);

                reader.Close();
            }

            return dt;
        }

        /// <summary>
        /// Retrieve All Tasks
        /// </summary>
        /// <param name="task">BCC Task</param>
        /// <returns>DataTable</returns>
        public static DataTable RetrieveTasks(BCCTask task)
        {
            DataTable dt = null;
            SqlParameter param = null;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskList_Tasks]", connection);
                command.CommandType = CommandType.StoredProcedure;

                if (task.AssignedToUser == string.Empty)
                {
                    param = new SqlParameter("@assignedToUserName", DBNull.Value);
                }
                else
                {
                    param = new SqlParameter("@assignedToUserName", task.AssignedToUser);
                }

                command.Parameters.Add(param);

                if (task.TaskStatus == string.Empty)
                {
                    param = new SqlParameter("@taskStatus", DBNull.Value);
                }
                else
                {
                    param = new SqlParameter("@taskStatus", task.TaskStatus);
                }
                
                command.Parameters.Add(param);

                param = new SqlParameter("@showFlag", task.TaskVisible);
                command.Parameters.Add(param);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dt = new DataTable();
                dt.Load(reader);

                reader.Close();
            }

            return dt;
        }


        public static void DeactivateTask(BCCTask task, bool hardDelete)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskList_DeactivateTask]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@taskID", task.TaskID);
                command.Parameters.Add(param);

                param = new SqlParameter("@updatedbyUser", task.UpdatedbyUser);
                command.Parameters.Add(param);

                int flag = 0;

                if (hardDelete)
                {
                    flag = 1;
                }

                param = new SqlParameter("@deleteFlag", flag);
                command.Parameters.Add(param);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static void UpdateTaskStatus(BCCTask task)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskList_UpdateTaskStatus]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@taskID", task.TaskID);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskStatus", task.TaskStatus);
                command.Parameters.Add(param);

                param = new SqlParameter("@updatedbyUser", task.UpdatedbyUser);
                command.Parameters.Add(param);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static XmlDocument WriteToXml(DataTable table)
        {
            XmlDocument xDoc = null;

            if (table != null)
            {
                table.TableName = "Task";
                StringWriter writer = new StringWriter();
                table.WriteXml(writer);

                xDoc = new XmlDocument();
                xDoc.LoadXml(writer.ToString());
            }

            return xDoc;
        }

    }
    
    public class BCCTaskSystemMessage
    {
        private string errorCode = string.Empty;
        private string errorMessage = string.Empty;

        public BCCTaskSystemMessage()
        {

        }

        public BCCTaskSystemMessage(string errorCode, string errorMessage)
        {
            this.errorCode = errorCode;
            this.errorMessage = errorMessage;
        }

        public string ErrorCode
        {
            get
            {
                return errorCode;
            }

            set
            {
                errorCode = value;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }

            set
            {
                errorMessage = value;
            }
        }
    }

    public class BCCTaskEffort
    {
        #region BCCTaskEffort Properties
        private Guid _taskId = Guid.Empty;
        private string _taskRef = string.Empty;

        private DateTime _taskEffortDate = System.DateTime.Today;
        private string _taskStatus = Enum.GetName(typeof(BCCTask.TaskState), BCCTask.TaskState.Created);
        private double _taskDuration = 4.0; // Default task duration is 4 hrs.
        private string _taskAssignedToUserName = string.Empty;
        private string _taskRemark = string.Empty;

        private DateTime _taskReportStart = System.DateTime.UtcNow.Subtract(new TimeSpan(7, 0, 0, 0)).Date;
        private DateTime _taskReportEnd = System.DateTime.UtcNow.Date;
        private string _projectName = string.Empty;

        public Guid TaskID
        {
            get
            {
                return _taskId;
            }
            set
            {
                _taskId = value;
            }
        }

        public string TaskRef
        {
            get
            {
                return _taskRef;
            }
            set
            {
                _taskRef = value;
            }
        }

        public string TaskStatus
        {
            get
            {
                return _taskStatus;
            }
            set
            {
                _taskStatus = value;
            }
        }

        public string ProjectName
        {
            get
            {
                return _projectName;
            }
            set
            {
                _projectName = value;
            }
        }

        public double TaskEffort
        {
            get
            {
                return _taskDuration;
            }
            set
            {
                _taskDuration = value;
            }
        }

        public string TaskAssignedToUserName
        {
            get
            {
                return _taskAssignedToUserName;
            }
            set
            {
                _taskAssignedToUserName = value;
            }
        }

        public DateTime TaskEffortDate
        {
            get
            {
                return _taskEffortDate;
            }
            set
            {
                _taskEffortDate = value;
            }
        }

        public DateTime ReportStartDate
        {
            get
            {
                return _taskReportStart;
            }
            set
            {
                _taskReportStart = value;
            }
        }

        public DateTime ReportEndDate
        {
            get
            {
                return _taskReportEnd;
            }
            set
            {
                _taskReportEnd = value;
            }
        }

        public string TaskRemark
        {
            get
            {
                return _taskRemark;
            }
            set
            {
                _taskRemark = value;
            }
        }
        #endregion 

        /// <summary>
        /// Retrieve All Tasks whatever the case may be.
        /// </summary>
        /// <param name="task">BCC Task</param>
        /// <returns>DataTable</returns>
        public static DataTable RetrieveAllTaskEfforts(BCCTaskEffort taskEffort)
        {
            DataTable dt = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskEfforts_Efforts]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@taskID", taskEffort.TaskID);
                command.Parameters.Add(param);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dt = new DataTable();
                dt.Load(reader);

                reader.Close();
            }

            return dt;
        }

        /// <summary>
        /// Retrieve All Tasks whatever the case may be.
        /// </summary>
        /// <param name="task">BCC Task</param>
        /// <returns>DataTable</returns>
        public static void CreateTaskEffort(BCCTaskEffort taskEffort)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskEfforts_Insert]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@taskID", taskEffort.TaskID);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskStatus", taskEffort.TaskStatus);
                command.Parameters.Add(param);

                param = new SqlParameter("@updatedbyUser", taskEffort.TaskAssignedToUserName);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskDate", taskEffort.TaskEffortDate);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskEffort", taskEffort.TaskEffort);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskRemark", taskEffort.TaskRemark);
                command.Parameters.Add(param);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="taskEffort"></param>
        public static void UpdateTaskEffort(BCCTaskEffort taskEffort)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskEfforts_Update]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@taskID", taskEffort.TaskID);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskRef", taskEffort.TaskRef);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskStatus", taskEffort.TaskStatus);
                command.Parameters.Add(param);

                param = new SqlParameter("@updatedbyUser", taskEffort.TaskAssignedToUserName);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskDate", taskEffort.TaskEffortDate);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskEffort", taskEffort.TaskEffort);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskRemark", taskEffort.TaskRemark);
                command.Parameters.Add(param);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Remove task efforts
        /// </summary>
        /// <param name="taskEffort"></param>
        /// <returns></returns>
        public static void RemoveTaskEfforts(BCCTaskEffort taskEffort)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskEfforts_Delete]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@taskID", taskEffort.TaskID);
                command.Parameters.Add(param);

                param = new SqlParameter("@taskRef", taskEffort.TaskRef);
                command.Parameters.Add(param);

                //@updatedbyUser
                param = new SqlParameter("@updatedbyUser", taskEffort.TaskAssignedToUserName);
                command.Parameters.Add(param);

                connection.Open();

                command.ExecuteNonQuery();

            }
        }

        /// <summary>
        /// Task Effort Report
        /// </summary>
        /// <param name="effort"></param>
        public static DataTable ReportTaskEfforts(BCCTaskEffort effort)
        {
            DataTable dt = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskEfforts_TaskEffortReport]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@assignedToUser", effort.TaskAssignedToUserName);
                command.Parameters.Add(param);

                param = new SqlParameter("@reportStartDate", effort.ReportStartDate);
                command.Parameters.Add(param);

                param = new SqlParameter("@reportEndDate", effort.ReportEndDate);
                command.Parameters.Add(param);

                if (effort.TaskStatus != null)
                {
                    param = new SqlParameter("@taskStatus", effort.TaskStatus);
                    command.Parameters.Add(param);
                }
                else
                {
                    param = new SqlParameter("@taskStatus", DBNull.Value);
                    command.Parameters.Add(param);
                }

                connection.Open();

                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                dt = new DataTable();
                dt.Load(reader);

                reader.Close();
            }

            return dt;
        }

        /// <summary>
        /// Task Effort Report
        /// </summary>
        /// <param name="effort"></param>
        public static DataTable ReportProjectEfforts(BCCTaskEffort effort)
        {
            DataTable dt = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_TaskEfforts_ProjectTaskEffort]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@projectName", effort.ProjectName);
                command.Parameters.Add(param);

                param = new SqlParameter("@reportStartDate", effort.ReportStartDate);
                command.Parameters.Add(param);

                param = new SqlParameter("@reportEndDate", effort.ReportEndDate);
                command.Parameters.Add(param);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                dt = new DataTable();
                dt.Load(reader);

                reader.Close();
            }

            return dt;
        }
    }
}
