'use client';

import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { budgetApi, categoryApi } from '@/lib/api/services';
import { Wallet, Plus, Trash2 } from 'lucide-react';

export default function BudgetPage() {
  const queryClient = useQueryClient();
  const [showSalaryForm, setShowSalaryForm] = useState(false);
  const [showBudgetForm, setShowBudgetForm] = useState(false);
  const [salary, setSalary] = useState('');
  const [budgetForm, setBudgetForm] = useState({
    categoryId: '',
    allocatedAmount: '',
    description: '',
  });

  const { data: overview, isLoading } = useQuery({
    queryKey: ['budgetOverview'],
    queryFn: budgetApi.getOverview,
  });

  const { data: categories } = useQuery({
    queryKey: ['categories'],
    queryFn: categoryApi.getCategories,
  });

  const setSalaryMutation = useMutation({
    mutationFn: (amount: number) => budgetApi.setSalary(amount),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['budgetOverview'] });
      setShowSalaryForm(false);
      setSalary('');
    },
  });

  const createBudgetMutation = useMutation({
    mutationFn: budgetApi.createBudget,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['budgetOverview'] });
      setShowBudgetForm(false);
      setBudgetForm({ categoryId: '', allocatedAmount: '', description: '' });
    },
  });

  const deleteBudgetMutation = useMutation({
    mutationFn: budgetApi.deleteBudget,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['budgetOverview'] });
    },
  });

  const handleSetSalary = (e: React.FormEvent) => {
    e.preventDefault();
    setSalaryMutation.mutate(parseFloat(salary));
  };

  const handleCreateBudget = (e: React.FormEvent) => {
    e.preventDefault();
    createBudgetMutation.mutate({
      categoryId: budgetForm.categoryId,
      allocatedAmount: parseFloat(budgetForm.allocatedAmount),
      description: budgetForm.description,
    });
  };

  if (isLoading) return <div>Loading...</div>;

  const getProgressColor = (spent: number, allocated: number) => {
    const percentage = (spent / allocated) * 100;
    if (percentage >= 100) return 'bg-red-500';
    if (percentage >= 80) return 'bg-yellow-500';
    return 'bg-green-500';
  };

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold">Budget Management</h1>
        <button
          onClick={() => setShowSalaryForm(!showSalaryForm)}
          className="btn-primary flex items-center space-x-2"
        >
          <Wallet className="w-5 h-5" />
          <span>Set Salary</span>
        </button>
      </div>

      {showSalaryForm && (
        <div className="stat-card">
          <h2 className="text-xl font-bold mb-4">Set Monthly Salary</h2>
          <form onSubmit={handleSetSalary} className="space-y-4">
            <div>
              <label className="block text-sm font-medium mb-2">Monthly Salary (₹)</label>
              <input
                type="number"
                value={salary}
                onChange={(e) => setSalary(e.target.value)}
                className="input"
                placeholder="50000"
                required
              />
            </div>
            <div className="flex space-x-3">
              <button type="submit" className="btn-primary">
                {setSalaryMutation.isPending ? 'Saving...' : 'Save Salary'}
              </button>
              <button type="button" onClick={() => setShowSalaryForm(false)} className="btn-secondary">
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div className="stat-card bg-gradient-to-br from-blue-500 to-indigo-600 text-white">
          <h3 className="text-sm font-medium opacity-90">Monthly Salary</h3>
          <p className="text-3xl font-bold mt-2">₹{overview?.monthlySalary.toFixed(2) || '0.00'}</p>
        </div>
        <div className="stat-card bg-gradient-to-br from-purple-500 to-pink-600 text-white">
          <h3 className="text-sm font-medium opacity-90">Total Allocated</h3>
          <p className="text-3xl font-bold mt-2">₹{overview?.totalAllocated.toFixed(2) || '0.00'}</p>
        </div>
        <div className="stat-card bg-gradient-to-br from-orange-500 to-red-600 text-white">
          <h3 className="text-sm font-medium opacity-90">Total Spent</h3>
          <p className="text-3xl font-bold mt-2">₹{overview?.totalSpent.toFixed(2) || '0.00'}</p>
        </div>
        <div className="stat-card bg-gradient-to-br from-green-500 to-emerald-600 text-white">
          <h3 className="text-sm font-medium opacity-90">Remaining</h3>
          <p className="text-3xl font-bold mt-2">₹{overview?.remaining.toFixed(2) || '0.00'}</p>
        </div>
      </div>

      <button onClick={() => setShowBudgetForm(!showBudgetForm)} className="btn-primary flex items-center space-x-2">
        <Plus className="w-5 h-5" />
        <span>Add Budget Allocation</span>
      </button>

      {showBudgetForm && (
        <div className="stat-card">
          <h2 className="text-xl font-bold mb-4">Add Budget Allocation</h2>
          <form onSubmit={handleCreateBudget} className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium mb-2">Category</label>
                <select
                  value={budgetForm.categoryId}
                  onChange={(e) => setBudgetForm({ ...budgetForm, categoryId: e.target.value })}
                  className="input"
                  required
                >
                  <option value="">Select category</option>
                  {categories?.map((cat) => (
                    <option key={cat.id} value={cat.id}>
                      {cat.icon} {cat.name}
                    </option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">Allocated Amount (₹)</label>
                <input
                  type="number"
                  value={budgetForm.allocatedAmount}
                  onChange={(e) => setBudgetForm({ ...budgetForm, allocatedAmount: e.target.value })}
                  className="input"
                  placeholder="7000"
                  required
                />
              </div>
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Description</label>
              <input
                type="text"
                value={budgetForm.description}
                onChange={(e) => setBudgetForm({ ...budgetForm, description: e.target.value })}
                className="input"
                placeholder="Home Rent"
                required
              />
            </div>
            <div className="flex space-x-3">
              <button type="submit" className="btn-primary">
                {createBudgetMutation.isPending ? 'Adding...' : 'Add Budget'}
              </button>
              <button type="button" onClick={() => setShowBudgetForm(false)} className="btn-secondary">
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

      <div className="stat-card">
        <h2 className="text-xl font-bold mb-4">Budget Allocations</h2>
        {overview?.budgets && overview.budgets.length > 0 ? (
          <div className="space-y-4">
            {overview.budgets.map((budget) => {
              const percentage = (budget.spentAmount / budget.allocatedAmount) * 100;
              return (
                <div key={budget.id} className="p-4 bg-gray-50 rounded-xl">
                  <div className="flex justify-between items-start mb-3">
                    <div className="flex items-center space-x-3">
                      <span className="text-3xl">{budget.categoryIcon}</span>
                      <div>
                        <p className="font-bold text-lg">{budget.description}</p>
                        <p className="text-sm text-gray-600">{budget.categoryName}</p>
                      </div>
                    </div>
                    <button
                      onClick={() => deleteBudgetMutation.mutate(budget.id)}
                      className="text-red-600 hover:text-red-800"
                    >
                      <Trash2 className="w-5 h-5" />
                    </button>
                  </div>
                  <div className="space-y-2">
                    <div className="flex justify-between text-sm">
                      <span>Spent: ₹{budget.spentAmount.toFixed(2)}</span>
                      <span>Budget: ₹{budget.allocatedAmount.toFixed(2)}</span>
                    </div>
                    <div className="w-full bg-gray-200 rounded-full h-3">
                      <div
                        className={`h-3 rounded-full transition-all ${getProgressColor(
                          budget.spentAmount,
                          budget.allocatedAmount
                        )}`}
                        style={{ width: `${Math.min(percentage, 100)}%` }}
                      />
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className={percentage >= 100 ? 'text-red-600 font-bold' : 'text-gray-600'}>
                        {percentage.toFixed(1)}% used
                      </span>
                      <span className={budget.remainingAmount < 0 ? 'text-red-600 font-bold' : 'text-green-600'}>
                        ₹{budget.remainingAmount.toFixed(2)} remaining
                      </span>
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        ) : (
          <p className="text-gray-500 text-center py-12">
            No budget allocations yet. Click "Add Budget Allocation" to get started!
          </p>
        )}
      </div>
    </div>
  );
}
