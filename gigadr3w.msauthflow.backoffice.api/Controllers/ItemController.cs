using gigadr3w.msauthflow.authenticator.iterator.Configurations;
using gigadr3w.msauthflow.backoffice.api.Requests;
using gigadr3w.msauthflow.backoffice.api.Responses;
using gigadr3w.msauthflow.backoffice.iterator.Models;
using gigadr3w.msauthflow.backoffice.iterator.Services;
using gigadr3w.msauthflow.common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace gigadr3w.msauthflow.backoffice.api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtTokenConfiguration.DEFAULT_SCHEMA)]
    public class ItemController : Controller
    {
        private readonly IItemService _itemService;
        private readonly ILogger<ItemController> _logger;

        public ItemController(IItemService itemService,
            ILogger<ItemController> logger)
            => (_itemService, _logger) = (itemService, logger);

        [HttpGet]
        [Authorize(Roles = "BackofficeRead")]
        [Route("List")]
        [SwaggerOperation(
            Summary = "Get the list of items",
            Description = "An authenticated request that require the items",
            OperationId = "ItemsList",
            Tags = new[] { "Items" }
        )]
        [SwaggerResponse(200, "Return the items list", typeof(List<GetItemResponse>))]
        [SwaggerResponse(404, "No items found")]
        [SwaggerResponse(401, "Invalid credentials")]
        [SwaggerResponse(403, "Invalid permissions")]
        [SwaggerResponse(415, "Invalid format")]
        [SwaggerResponse(500, "Server error")]
        public async Task<IActionResult> ListItems()
            => Ok((await _itemService.List()).Select(i => new GetItemResponse { Id = i.Id, Name = i.Name, Description = i.Description, Value = i.Value }));

        [HttpGet("{Id}")]
        [Authorize(Roles = "BackofficeRead")]
        [SwaggerOperation(
            Summary = "Get the item if present",
            Description = "An authenticated request that require the item",
            OperationId = "Item",
            Tags = new[] { "Items" }
        )]
        [SwaggerResponse(200, "Return the item", typeof(GetItemResponse))]
        [SwaggerResponse(404, "No item found")]
        [SwaggerResponse(401, "Invalid credentials")]
        [SwaggerResponse(403, "Invalid permissions")]
        [SwaggerResponse(415, "Invalid format")]
        [SwaggerResponse(500, "Server error")]
        public async Task<IActionResult> Get([FromRoute, Required(ErrorMessage = "Id is mandatory")] int Id)
        {
            ItemModel model = await _itemService.GetOrThrow(Id);
            GetItemResponse response = new() 
            { 
                Id = model.Id, 
                Name = model.Name, 
                Description = model.Description, 
                Value = model.Value 
            };
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "BackofficeWrite")]
        [SwaggerOperation(
            Summary = "Add a new item",
            Description = "An authenticated request that add the item",
            OperationId = "AddItem",
            Tags = new[] { "Items" }
        )]
        [SwaggerResponse(201, "Item has been succesfully added", typeof(GetItemResponse))]
        [SwaggerResponse(401, "Invalid credentials")]
        [SwaggerResponse(403, "Invalid permissions")]
        [SwaggerResponse(415, "Invalid format")]
        [SwaggerResponse(500, "Server error")]
        public async Task<IActionResult> Post([FromBody] AddItemRequest request)
        {
            ItemModel model = new ()
            { 
                Name = request.Name, 
                Description = request.Description, 
                Value = request.Value 
            };

            ItemModel created = await _itemService.Add(model);
            return CreatedAtAction("Get", new { Id = created.Id }, created);            
        }

        [HttpPatch]
        [Authorize(Roles = "BackofficeWrite")]
        [SwaggerOperation(
            Summary = "Update an item if present",
            Description = "An authenticated request that update the specified item",
            OperationId = "UpdateItem",
            Tags = new[] { "Items" }
        )]
        [SwaggerResponse(201, "Item has been succesfully updated", typeof(GetItemResponse))]
        [SwaggerResponse(401, "Invalid credentials")]
        [SwaggerResponse(403, "Invalid permissions")]
        [SwaggerResponse(415, "Invalid format")]
        [SwaggerResponse(500, "Server error")]
        public async Task<IActionResult> Patch([FromBody] UpdateItemRequest request)
        {
            try
            {
                ItemModel model = new ()
                { 
                    Id = request.Id, 
                    Name = request.Name, 
                    Description = request.Description, 
                    Value = request.Value 
                };

                ItemModel updated = await _itemService.UpdateOrThrow(model);
                return CreatedAtAction("Get", new { Id = updated.Id }, updated);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{Id}")]
        [Authorize(Roles = "BackofficeWrite")]
        [SwaggerOperation(
            Summary = "Delete an item if present",
            Description = "An authenticated request that delete the specified item",
            OperationId = "DeleteItem",
            Tags = new[] { "Items" }
        )]
        [SwaggerResponse(201, "Item has been succesfully deleted", typeof(GetItemResponse))]
        [SwaggerResponse(401, "Invalid credentials")]
        [SwaggerResponse(403, "Invalid permissions")]
        [SwaggerResponse(415, "Invalid format")]
        [SwaggerResponse(500, "Server error")]
        public async Task<IActionResult> Delete([FromRoute, Required(ErrorMessage = "Id is mandatory")] int Id)
        {
            try
            {
                await _itemService.DeleteOrThrow(Id);
                return StatusCode(204);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
