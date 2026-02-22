using System.Security.Claims;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseResponse>>> GetExpenses([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var expenses = await _expenseService.GetExpensesAsync(GetUserId(), startDate, endDate);
        return Ok(expenses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseResponse>> GetExpense(Guid id)
    {
        var expense = await _expenseService.GetExpenseByIdAsync(GetUserId(), id);
        if (expense == null) return NotFound();
        return Ok(expense);
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseResponse>> CreateExpense([FromBody] CreateExpenseRequest request)
    {
        try
        {
            var expense = await _expenseService.CreateExpenseAsync(GetUserId(), request);
            return CreatedAtAction(nameof(GetExpense), new { id = expense.Id }, expense);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ExpenseResponse>> UpdateExpense(Guid id, [FromBody] UpdateExpenseRequest request)
    {
        try
        {
            var expense = await _expenseService.UpdateExpenseAsync(GetUserId(), id, request);
            return Ok(expense);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExpense(Guid id)
    {
        await _expenseService.DeleteExpenseAsync(GetUserId(), id);
        return NoContent();
    }

    [HttpGet("summary/{year}/{month}")]
    public async Task<ActionResult<MonthlySummaryResponse>> GetMonthlySummary(int year, int month)
    {
        var summary = await _expenseService.GetMonthlySummaryAsync(GetUserId(), year, month);
        return Ok(summary);
    }
}
