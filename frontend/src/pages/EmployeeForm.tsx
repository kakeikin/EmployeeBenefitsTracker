import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useCreateEmployee, useEmployee, useUpdateEmployee } from '../api/employees';
import { FormField, Input, Select } from '../components/FormField';
import { DEPARTMENTS, EMPLOYMENT_STATUSES } from '../utils/formatters';
import type { CreateEmployeeRequest } from '../types/employee';

const empty: CreateEmployeeRequest = {
  firstName: '',
  lastName: '',
  email: '',
  department: 'Engineering',
  employmentStatus: 'Active',
};

export default function EmployeeForm() {
  const { id } = useParams();
  const isEdit = Boolean(id);
  const navigate = useNavigate();
  const [form, setForm] = useState<CreateEmployeeRequest>(empty);
  const [errors, setErrors] = useState<Partial<Record<keyof CreateEmployeeRequest | 'api', string>>>({});

  const { data: existing } = useEmployee(Number(id ?? 0));
  const createMutation = useCreateEmployee();
  const updateMutation = useUpdateEmployee(Number(id ?? 0));

  useEffect(() => {
    if (existing && isEdit) {
      setForm({
        firstName: existing.firstName,
        lastName: existing.lastName,
        email: existing.email,
        department: existing.department,
        employmentStatus: existing.employmentStatus,
      });
    }
  }, [existing, isEdit]);

  const validate = (): boolean => {
    const errs: Partial<Record<keyof CreateEmployeeRequest | 'api', string>> = {};
    if (!form.firstName.trim()) errs.firstName = 'First name is required.';
    if (!form.lastName.trim()) errs.lastName = 'Last name is required.';
    if (!form.email.trim()) errs.email = 'Email is required.';
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) errs.email = 'Invalid email format.';
    if (!form.department) errs.department = 'Department is required.';
    setErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;
    try {
      if (isEdit) {
        await updateMutation.mutateAsync(form);
        navigate(`/employees/${id}`);
      } else {
        const created = await createMutation.mutateAsync(form);
        navigate(`/employees/${created.id}`);
      }
    } catch (err: unknown) {
      const msg =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        'An error occurred.';
      setErrors(prev => ({ ...prev, api: msg }));
    }
  };

  const set =
    (field: keyof CreateEmployeeRequest) =>
    (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) =>
      setForm(prev => ({ ...prev, [field]: e.target.value }));

  const isPending = createMutation.isPending || updateMutation.isPending;

  return (
    <div className="min-h-screen bg-gray-50 flex items-start justify-center pt-12 px-4">
      <div className="bg-white rounded-xl border border-gray-200 shadow-sm w-full max-w-lg p-8">
        <div className="mb-6">
          <button onClick={() => navigate(-1)} className="text-sm text-gray-400 hover:text-gray-600 mb-4 block">
            ← Back
          </button>
          <h1 className="text-xl font-semibold text-gray-900">
            {isEdit ? 'Edit Employee' : 'Add Employee'}
          </h1>
        </div>

        {errors.api && (
          <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded text-sm text-red-700">
            {errors.api}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <FormField label="First Name" error={errors.firstName}>
              <Input
                value={form.firstName}
                onChange={set('firstName')}
                error={Boolean(errors.firstName)}
                placeholder="Alice"
              />
            </FormField>
            <FormField label="Last Name" error={errors.lastName}>
              <Input
                value={form.lastName}
                onChange={set('lastName')}
                error={Boolean(errors.lastName)}
                placeholder="Smith"
              />
            </FormField>
          </div>
          <FormField label="Email" error={errors.email}>
            <Input
              type="email"
              value={form.email}
              onChange={set('email')}
              error={Boolean(errors.email)}
              placeholder="alice@example.com"
            />
          </FormField>
          <FormField label="Department" error={errors.department}>
            <Select
              value={form.department}
              onChange={set('department')}
              options={DEPARTMENTS.map(d => ({ value: d, label: d }))}
            />
          </FormField>
          <FormField label="Employment Status">
            <Select
              value={form.employmentStatus}
              onChange={set('employmentStatus')}
              options={EMPLOYMENT_STATUSES.map(s => ({ value: s, label: s }))}
            />
          </FormField>

          <div className="flex gap-3 pt-2">
            <button
              type="button"
              onClick={() => navigate(-1)}
              className="flex-1 px-4 py-2 border border-gray-300 rounded-lg text-sm text-gray-700 hover:bg-gray-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isPending}
              className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-50"
            >
              {isPending ? 'Saving...' : isEdit ? 'Save Changes' : 'Create Employee'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
