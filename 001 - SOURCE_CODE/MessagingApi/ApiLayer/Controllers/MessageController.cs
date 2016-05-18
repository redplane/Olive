using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ApiLayer.Context;
using ApiLayer.Models;
using ApiLayer.ViewModel;

namespace ApiLayer.Controllers
{
    public class MessageController : Controller
    {
        private readonly MessagingContext _context;

        /// <summary>
        /// Initialize an instance of MessageController class.
        /// </summary>
        public MessageController()
        {
            _context = new MessagingContext();
        }

        /// <summary>
        /// Retrieve message by using conditions.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Retrieve(MessageGetViewModel model)
        {
            // Invalid model.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                return Json(errors);
            }

            // Invalid time.
            if (model.From != null && model.To != null)
            {
                if (model.From > model.To)
                    return Json(new {});
            }
            
            // Filter result by using sender, recipient name and message status.
            var results =
                _context.PrivateMessages.Where(
                    x =>
                        !string.IsNullOrEmpty(x.Sender) && !string.IsNullOrEmpty(x.Recipient) 
                        && x.Sender.Equals(model.Sender) && x.Recipient.Equals(model.Recipient)
                        && x.IsReceived == model.IsReceived);
            
            // Date
            if (model.From != null)
                results = results.Where(x => x.Sent >= model.From);

            if (model.To != null)
                results = results.Where(x => x.Sent <= model.To);
            
            // Date order.
            if (model.Order == 1)
                results = results.OrderBy(x => x.Sent);
            else if (model.Order == 2)
                results = results.OrderByDescending(x => x.Sent);
            
            return Json(results);

        }

        /// <summary>
        /// Send message using parameters.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Send(MessageSendViewModel model)
        {
            // Invalid model.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Response.StatusCode = (int) HttpStatusCode.Forbidden;
                return Json(errors);
            }

            try
            {
                var message = new PrivateMessage();
                message.Sender = model.Sender;
                message.Recipient = model.Recipient;
                message.Content = model.Content;
                message.Sent = DateTime.Now.Ticks;

                _context.PrivateMessages.Add(message);
                await _context.SaveChangesAsync();

                Response.StatusCode = (int) HttpStatusCode.OK;
                return Json(message);
            }
            catch (DbEntityValidationException e)
            {
                //foreach (var eve in e.EntityValidationErrors)
                //{
                //    Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                //        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                //    foreach (var ve in eve.ValidationErrors)
                //    {
                //        Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                //            ve.PropertyName, ve.ErrorMessage);
                //    }
                //}
                //throw;
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);
            }
            
            
        }

        /// <summary>
        /// Update message status.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> Update(MessagePutViewModel model)
        {
            // Invalid model.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(errors);
            }

            var a = await _context.PrivateMessages.Take(5).ToListAsync();

            // Retrieve message by using id and hasn't been received.
            var message = await _context.PrivateMessages.Where(x => x.Id == model.Id && !x.IsReceived).FirstOrDefaultAsync();
            if (message == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            message.IsReceived = true;
            _context.PrivateMessages.AddOrUpdate(message);
            await _context.SaveChangesAsync();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}