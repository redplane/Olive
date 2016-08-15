using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Olives.Interfaces;
using Olives.ViewModels.Filter;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels.Response;

namespace Olives.Repositories
{
    public class RepositoryMessage : IRepositoryMessage
    {
        #region Properties

        private readonly IOliveDataContext _dataContext;

        #endregion

        #region Constructor

        public RepositoryMessage(IOliveDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Send a message asychronously.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public async Task<Message> BroadcastMessageAsync(Message initializer)
        {
            // Initialize a message to database.
            var context = _dataContext.Context;
            context.Messages.Add(initializer);

            // Save changes.
            await context.SaveChangesAsync();

            return initializer;
        }

        /// <summary>
        ///     Filter messages by using specific conditions asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseMessageFilter> FilterMessagesAsync(FilterMessageViewModel filter)
        {
            // By default, take all the messages.
            var context = _dataContext.Context;
            IQueryable<Message> messages = context.Messages;

            // Filter the messages.
            messages = FilterMessages(messages, filter);

            // Record sorting.
            switch (filter.Direction)
            {
                case SortDirection.Decending:
                    switch (filter.Sort)
                    {
                        case MessageFilterSort.Created:
                            messages = messages.OrderByDescending(x => x.Created);
                            break;
                        default:
                            messages = messages.OrderByDescending(x => x.IsSeen);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case MessageFilterSort.Created:
                            messages = messages.OrderBy(x => x.Created);
                            break;
                        default:
                            messages = messages.OrderBy(x => x.IsSeen);
                            break;
                    }
                    break;
            }

            // Response initialization.
            var response = new ResponseMessageFilter();
            response.Total = await messages.CountAsync();

            // Pagination is defined.
            if (filter.Records != null)
                messages = messages.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);

            response.Messages = messages;
            return response;
        }

        /// <summary>
        ///     Find a message by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Message> FindMessageAsync(int id)
        {
            var context = _dataContext.Context;
            return await context.Messages.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Make the message be seen.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<int> MakeMessagesSeen(FilterMessageViewModel filter)
        {
            // Database context initialization.
            var context = _dataContext.Context;

            // Take all the messages.
            IQueryable<Message> messages = context.Messages;

            // Begin the transaction.
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // Only owner can make message be seen.
                    filter.Mode = RecordFilterMode.RequesterIsOwner;
                    filter.IsSeen = false;

                    // Filter the message.
                    messages = FilterMessages(messages, filter);

                    // Change the status of messages.
                    await messages.ForEachAsync(x =>
                    {
                        x.IsSeen = true;
                    });

                    // Save the changes.
                    var records = await context.SaveChangesAsync();

                    return records;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Filter message by using specific conditions.
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private IQueryable<Message> FilterMessages(IQueryable<Message> messages, FilterMessageViewModel filter)
        {
            // Base on the mode of image filter to decide the role of requester.
            if (filter.Mode == RecordFilterMode.RequesterIsOwner)
            {
                messages = messages.Where(x => x.Recipient == filter.Requester);
                if (filter.Partner != null)
                    messages = messages.Where(x => x.Broadcaster == filter.Partner.Value);
            }
            else if (filter.Mode == RecordFilterMode.RequesterIsCreator)
            {
                messages = messages.Where(x => x.Broadcaster == filter.Requester);
                if (filter.Partner != null)
                    messages = messages.Where(x => x.Recipient == filter.Partner);
            }
            else
            {
                if (filter.Partner == null)
                    messages =
                        messages.Where(x => x.Broadcaster == filter.Requester || x.Recipient == filter.Requester);
                else
                    messages =
                        messages.Where(
                            x =>
                                (x.Broadcaster == filter.Requester && x.Recipient == filter.Partner.Value) ||
                                (x.Broadcaster == filter.Partner.Value && x.Recipient == filter.Requester));
            }

            // Created is defined.
            if (filter.MinCreated != null) messages = messages.Where(x => x.Created >= filter.MinCreated.Value);
            if (filter.MaxCreated != null) messages = messages.Where(x => x.Created <= filter.MaxCreated.Value);

            // Status is defined.
            if (filter.IsSeen != null)
                messages = messages.Where(x => x.IsSeen == filter.IsSeen.Value);

            return messages;
        } 

        #endregion
    }
}