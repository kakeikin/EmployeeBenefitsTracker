import { useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { useEmployee } from '../api/employees';
import {
  useCreateBenefit,
  useDeleteBenefit,
  useEmployeeBenefits,
  useUpdateBenefit,
} from '../api/benefits';
import Badge from '../components/Badge';
import Modal from '../components/Modal';
import { FormField, Select } from '../components/FormField';
import { BENEFIT_TYPES, ENROLLMENT_STATUSES, formatDate } from '../utils/formatters';
import type { BenefitType, EnrollmentStatus, CreateBenefitRequest } from '../types/benefit';

export default function EmployeeDetail() {
  const { id } = useParams();
  const empId = Number(id);
  const { data: employee, isLoading } = useEmployee(empId);
  const { data: benefits } = useEmployeeBenefits(empId);
  const createBenefit = useCreateBenefit(empId);
  const updateBenefit = useUpdateBenefit(empId);
  const deleteBenefit = useDeleteBenefit(empId);

  const [showAdd, setShowAdd] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [newBenefit, setNewBenefit] = useState<CreateBenefitRequest>({
    benefitType: 'Health',
    enrollmentStatus: 'Pending',
  });
  const [editStatus, setEditStatus] = useState<EnrollmentStatus>('Pending');
  const [addError, setAddError] = useState('');

  const existingTypes = new Set(benefits?.map(b => b.benefitType) ?? []);
  const availableTypes = BENEFIT_TYPES.filter(t => !existingTypes.has(t));

  const handleAddBenefit = async () => {
    setAddError('');
    try {
      await createBenefit.mutateAsync(newBenefit);
      setShowAdd(false);
      setNewBenefit({ benefitType: availableTypes[0] ?? 'Health', enrollmentStatus: 'Pending' });
    } catch (err: unknown) {
      const msg =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        'Failed to add benefit.';
      setAddError(msg);
    }
  };

  const openEdit = (id: number, currentStatus: EnrollmentStatus) => {
    setEditId(id);
    setEditStatus(currentStatus);
  };

  const handleUpdateBenefit = async () => {
    if (!editId) return;
    await updateBenefit.mutateAsync({ benefitId: editId, dto: { enrollmentStatus: editStatus } });
    setEditId(null);
  };

  if (isLoading) return <div className="p-8 text-center text-gray-500">Loading...</div>;
  if (!employee) return <div className="p-8 text-center text-red-500">Employee not found.</div>;

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-4xl mx-auto px-4 py-8">
        <div className="mb-6">
          <Link to="/" className="text-sm text-gray-400 hover:text-gray-600">← Back to List</Link>
        </div>

        <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-6 mb-6">
          <div className="flex items-start justify-between">
            <div>
              <h1 className="text-2xl font-bold text-gray-900">
                {employee.firstName} {employee.lastName}
              </h1>
              <p className="text-gray-500 mt-1">{employee.email}</p>
            </div>
            <Link
              to={`/employees/${empId}/edit`}
              className="px-4 py-2 border border-gray-300 rounded-lg text-sm text-gray-700 hover:bg-gray-50"
            >
              Edit
            </Link>
          </div>
          <div className="mt-4 grid grid-cols-2 gap-4 text-sm">
            <div>
              <span className="text-gray-500">Department:</span>
              <span className="font-medium ml-1">{employee.department}</span>
            </div>
            <div>
              <span className="text-gray-500">Status:</span>
              <span className="ml-1"><Badge value={employee.employmentStatus} /></span>
            </div>
            <div>
              <span className="text-gray-500">Joined:</span>
              <span className="font-medium ml-1">{formatDate(employee.createdAt)}</span>
            </div>
          </div>
        </div>

        <div className="bg-white rounded-xl border border-gray-200 shadow-sm">
          <div className="flex items-center justify-between px-6 py-4 border-b">
            <h2 className="text-lg font-semibold text-gray-800">Benefits Enrollment</h2>
            {availableTypes.length > 0 && (
              <button
                onClick={() => {
                  setShowAdd(true);
                  setNewBenefit({ benefitType: availableTypes[0], enrollmentStatus: 'Pending' });
                }}
                className="bg-blue-600 text-white px-3 py-1.5 rounded-lg text-sm hover:bg-blue-700"
              >
                + Add Benefit
              </button>
            )}
          </div>

          <table className="w-full text-sm">
            <thead className="bg-gray-50 text-gray-600 uppercase text-xs tracking-wide">
              <tr>
                {['Benefit Type', 'Status', 'Effective Date', 'Notes', 'Actions'].map(h => (
                  <th key={h} className="px-4 py-3 text-left font-medium">{h}</th>
                ))}
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {(!benefits || benefits.length === 0) && (
                <tr>
                  <td colSpan={5} className="px-4 py-6 text-center text-gray-400">
                    No benefits enrolled.
                  </td>
                </tr>
              )}
              {benefits?.map(b => (
                <tr key={b.id} className="hover:bg-gray-50">
                  <td className="px-4 py-3 font-medium">{b.benefitType}</td>
                  <td className="px-4 py-3"><Badge value={b.enrollmentStatus} /></td>
                  <td className="px-4 py-3 text-gray-500">{formatDate(b.effectiveDate)}</td>
                  <td className="px-4 py-3 text-gray-500 max-w-xs truncate">{b.notes ?? '—'}</td>
                  <td className="px-4 py-3">
                    <div className="flex gap-2">
                      <button
                        onClick={() => openEdit(b.id, b.enrollmentStatus as EnrollmentStatus)}
                        className="text-blue-600 hover:underline text-xs"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => deleteBenefit.mutate(b.id)}
                        className="text-red-400 hover:text-red-600 text-xs"
                      >
                        Remove
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {showAdd && (
        <Modal title="Add Benefit" onClose={() => setShowAdd(false)}>
          <div className="space-y-4">
            {addError && <p className="text-sm text-red-600">{addError}</p>}
            <FormField label="Benefit Type">
              <Select
                value={newBenefit.benefitType}
                onChange={e => setNewBenefit(p => ({ ...p, benefitType: e.target.value as BenefitType }))}
                options={availableTypes.map(t => ({ value: t, label: t }))}
              />
            </FormField>
            <FormField label="Status">
              <Select
                value={newBenefit.enrollmentStatus}
                onChange={e =>
                  setNewBenefit(p => ({ ...p, enrollmentStatus: e.target.value as EnrollmentStatus }))
                }
                options={ENROLLMENT_STATUSES.map(s => ({
                  value: s,
                  label: s === 'NotEnrolled' ? 'Not Enrolled' : s,
                }))}
              />
            </FormField>
            <div className="flex gap-3 pt-2">
              <button
                onClick={() => setShowAdd(false)}
                className="flex-1 px-4 py-2 border rounded-lg text-sm text-gray-700"
              >
                Cancel
              </button>
              <button
                onClick={handleAddBenefit}
                disabled={createBenefit.isPending}
                className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm disabled:opacity-50"
              >
                {createBenefit.isPending ? 'Adding...' : 'Add'}
              </button>
            </div>
          </div>
        </Modal>
      )}

      {editId && (
        <Modal title="Update Benefit Status" onClose={() => setEditId(null)}>
          <div className="space-y-4">
            <FormField label="Status">
              <Select
                value={editStatus}
                onChange={e => setEditStatus(e.target.value as EnrollmentStatus)}
                options={ENROLLMENT_STATUSES.map(s => ({
                  value: s,
                  label: s === 'NotEnrolled' ? 'Not Enrolled' : s,
                }))}
              />
            </FormField>
            <div className="flex gap-3 pt-2">
              <button
                onClick={() => setEditId(null)}
                className="flex-1 px-4 py-2 border rounded-lg text-sm text-gray-700"
              >
                Cancel
              </button>
              <button
                onClick={handleUpdateBenefit}
                disabled={updateBenefit.isPending}
                className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm disabled:opacity-50"
              >
                {updateBenefit.isPending ? 'Saving...' : 'Save'}
              </button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  );
}
