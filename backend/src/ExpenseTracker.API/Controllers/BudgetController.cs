using System.Security.Claims;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BudgetController : ControllerBase
{
    private readonly IBudgetService _budgetService;

    public BudgetController(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("salary")]
    public async Task<IActionResult> SetSalary([FromBody] SetSalaryRequest request)
    {
        await _budgetService.SetMonthlySalaryAsync(GetUserId(), request);
        return Ok(new { message = "Salary updated successfully" });
    }

    [HttpGet("overview")]
    public async Task<ActionResult<SalaryOverviewResponse>> GetOverview()
    {
        var overview = await _budgetService.GetSalaryOverviewAsync(GetUserId());
        return Ok(overview);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BudgetResponse>>> GetBudgets()
    {
        var budgets = await _budgetService.GetBudgetsAsync(GetUserId());
        return Ok(budgets);
    }

    [HttpPost]
    public async Task<ActionResult<BudgetResponse>> CreateBudget([FromBody] CreateBudgetRequest request)
    {
        try
        {
            var budget = await _budgetService.CreateBudgetAsync(GetUserId(), request);
            return Ok(budget);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BudgetResponse>> UpdateBudget(Guid id, [FromBody] UpdateBudgetRequest request)
    {
        try
        {
            var budget = await _budgetService.UpdateBudgetAsync(GetUserId(), id, request);
            return Ok(budget);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBudget(Guid id)
    {
        await _budgetService.DeleteBudgetAsync(GetUserId(), id);
        return NoContent();
    }
}
