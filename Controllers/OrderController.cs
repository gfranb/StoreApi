using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApi.DAL;
using StoreApi.Data;
using StoreApi.DTO;
using StoreApi.Models;
using System.Security.Claims;

namespace StoreApi.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly StoreDbContext context;
        private readonly IEmailSenderRepository emailSender;

        public OrderController(StoreDbContext dbContext, IEmailSenderRepository emailSender)
        {
            this.context = dbContext;
            this.emailSender = emailSender;
        }

        //Get endpoint to fetch all the orders from the db
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            using (var orderRepository = new OrderRepository(context))
            {
                var orders = await orderRepository.GetAll();
                return Ok(orders);
            }
        }

        //Get endpoint to fetch a unique order by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            using (var orderRepository = new OrderRepository(context))
            {
                Order? orders = await orderRepository.GetById(id);
                if (orders != null)
                {
                    return Ok(orders);
                }
                else
                {
                    return NotFound("Order not found");
                }
            }
        }

        //Get endpoint to fetch a unique order by id when is authenticated
        [HttpGet("my-orders")]
        [Authorize]
        public async Task<IActionResult> GetAuthenticatedUserOrders()
        {
            var stringId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            int jwtId = 0;

            if (stringId != null)
            {
                jwtId = Int32.Parse(stringId);
            }

            if (await context.Users.FindAsync(jwtId) != null)
            {
                using (var orderRepository = new OrderRepository(context))
                {
                    Order? orders = await orderRepository.GetById(jwtId);
                    if (orders != null)
                    {
                        return Ok(orders);
                    }
                    else
                    {
                        return NotFound("Order not found");
                    }
                }
            }
            else
            {
                return BadRequest("Error with the JWT");
            }
        }

        //Post endpoint to Add an order into the DB
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostOrder(OrderDTO request)
        {
            var stringId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            int jwtId = 0;

            if (stringId != null)
            {
                jwtId = Int32.Parse(stringId);
            }
            var currentUser = await context.Users.FindAsync(jwtId);
            if ( currentUser != null)
            {
                using (var orderRepository = new OrderRepository(context))
                {
                    request.UserID = jwtId;
                    var newOrder = await orderRepository.Insert(request);
                    if (newOrder != null)
                    {
                        await emailSender.SendEmailAsync(currentUser.Email, "Your order created!", "Hi, " + currentUser.Name + " your order is created succesfully with the number " + newOrder.Id);
                        return Created("Added", newOrder);
                    }
                    else
                    {
                        return BadRequest("Error creating the order, please verify the information");
                    }
                }
            }
            else
            {
                return BadRequest("Error with the JWT");
            }
            
        }

        //Put endpoint to update the data stored from a product
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, OrderDTO request)
        {
            using (var orderRepository = new OrderRepository(context))
            {
                Order? order = await orderRepository.Update(id, request);
                if (order != null)
                {
                    return Ok(order);
                }
                else
                {
                    return BadRequest("Error editing the order");
                }
            }
        }

        //Put endpoint to update the data stored from a product
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            using (var orderRepository = new OrderRepository(context))
            {
                if (await orderRepository.Delete(id))
                {
                    return Ok();
                }
                else
                {
                    return NotFound("Order not found");
                }
            }
        }

    }
}
