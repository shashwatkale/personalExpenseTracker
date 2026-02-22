'use client';

import { useMonthlySummary, useExpenses } from '@/hooks/useExpenses';
import { PieChart, Pie, Cell, ResponsiveContainer, Legend, Tooltip } from 'recharts';
import { format } from 'date-fns';

const COLORS = ['#EF4444', '#3B82F6', '#8B5CF6', '#EC4899', '#10B981', '#F59E0B', '#6B7280'];

export default function DashboardPage() {
  const currentDate = new Date();
  const year = currentDate.getFullYear();
  const month = currentDate.getMonth() + 1;

  const { data: summary, isLoading: summaryLoading } = useMonthlySummary(year, month);
  const { data: expenses, isLoading: expensesLoading } = useExpenses();

  if (summaryLoading || expensesLoading) {
    return <div>Loading dashboard...</div>;
  }

  const chartData = summary?.categoryBreakdown
    ? Object.entries(summary.categoryBreakdown).map(([name, value]) => ({
        name,
        value,
      }))
    : [];

  const recentExpenses = expenses?.slice(0, 5) || [];

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold">Dashboard</h1>
        <p className="text-gray-600">{format(currentDate, 'MMMM yyyy')}</p>
      </div>

      {/* Summary Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="card">
          <h3 className="text-sm font-medium text-gray-600">Total Expenses</h3>
          <p className="text-3xl font-bold mt-2">${summary?.totalExpenses.toFixed(2) || '0.00'}</p>
        </div>
        <div className="card">
          <h3 className="text-sm font-medium text-gray-600">Transactions</h3>
          <p className="text-3xl font-bold mt-2">{summary?.expenseCount || 0}</p>
        </div>
        <div className="card">
          <h3 className="text-sm font-medium text-gray-600">Categories</h3>
          <p className="text-3xl font-bold mt-2">
            {Object.keys(summary?.categoryBreakdown || {}).length}
          </p>
        </div>
      </div>

      {/* AI Insights */}
      {summary?.aiSummary && (
        <div className="card bg-gradient-to-r from-primary-50 to-blue-50">
          <h2 className="text-xl font-bold mb-4">💡 AI Insights</h2>
          <p className="text-gray-700 mb-4">{summary.aiSummary}</p>
          {summary.aiSuggestions && summary.aiSuggestions.length > 0 && (
            <div className="space-y-2">
              <h3 className="font-semibold text-gray-800">Suggestions:</h3>
              <ul className="space-y-2">
                {summary.aiSuggestions.map((suggestion, index) => (
                  <li key={index} className="text-sm text-gray-700 flex items-start">
                    <span className="mr-2">•</span>
                    <span>{suggestion}</span>
                  </li>
                ))}
              </ul>
            </div>
          )}
        </div>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Category Breakdown Chart */}
        <div className="card">
          <h2 className="text-xl font-bold mb-4">Spending by Category</h2>
          {chartData.length > 0 ? (
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={chartData}
                  cx="50%"
                  cy="50%"
                  labelLine={false}
                  label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
                  outerRadius={80}
                  fill="#8884d8"
                  dataKey="value"
                >
                  {chartData.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          ) : (
            <p className="text-gray-500 text-center py-12">No expenses yet</p>
          )}
        </div>

        {/* Recent Expenses */}
        <div className="card">
          <h2 className="text-xl font-bold mb-4">Recent Expenses</h2>
          {recentExpenses.length > 0 ? (
            <div className="space-y-3">
              {recentExpenses.map((expense) => (
                <div key={expense.id} className="flex justify-between items-center p-3 bg-gray-50 rounded-lg">
                  <div className="flex items-center space-x-3">
                    <span className="text-2xl">{expense.categoryIcon}</span>
                    <div>
                      <p className="font-medium">{expense.description}</p>
                      <p className="text-sm text-gray-600">{expense.categoryName}</p>
                    </div>
                  </div>
                  <p className="font-bold text-lg">${expense.amount.toFixed(2)}</p>
                </div>
              ))}
            </div>
          ) : (
            <p className="text-gray-500 text-center py-12">No expenses yet</p>
          )}
        </div>
      </div>
    </div>
  );
}
