using System.Threading;
using Shared.Enumerations;

namespace Shared.Models
{
    public class TaskDetail
    {
        #region Constructor

        /// <summary>
        ///     Initialize a task detail instance.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="cancellationTokenSource"></param>
        public TaskDetail(int id, TaskType type, CancellationTokenSource cancellationTokenSource)
        {
            Id = id;
            Type = type;
            CancellationTokenSource = cancellationTokenSource;
        }

        #endregion

        /// <summary>
        ///     Id of task.
        /// </summary>
        public int Id { get; }

        /// <summary>
        ///     Type of task
        /// </summary>
        public TaskType Type { get; }

        /// <summary>
        ///     Token which is used for cancelling task.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; }
    }
}