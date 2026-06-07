import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useDeleteEmployee, useEmployees } from '../api/employees';
import { useBenefitSummary } from '../api/benefits';
import Badge from '../components/Badge';
import Pagination from '../components/Pagination';
import SummaryCard from '../components/SummaryCard';
import { formatPercent, DEPARTMENTS, ENROLLMENT_STATUSES } from '../utils/formatters';

export default function EmployeeList() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [department, setDepartment] = useState('');
  const [benefitStatus, setBenefitStatus] = useState('');
  const [deleteConfirm, setDeleteConfirm] = useState<number | null>(null);

  const { data, isLoading, error } = useEmployees({
    page,
    pageSize: 10,
    search: search || undefined,
    department: department || undefined,
    benefitStatus: benefitStatus || undefined,
  });
  const { data: summary } = useBenefitSummary();
  const deleteMutation = useDeleteEmployee();

  const handleSearch = (v: string) => { setSearch(v); setPage(1); };
  const handleDept = (v: string) => { setDepartment(v); setPage(1); };
  const handleStatus = (v: string) => { setBenefitStatus(v); setPage(1); };

  const confirmDelete = async (id: number) => {
    await deleteMutation.mutateAsync(id);
    setDeleteConfirm(null);
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 py-8">
        <div className="flex items-center justify-between mb-8">
          <h1 className="text-2xl font-bold text-gray-900">Employee Benefits Tracker</h1>
          <Link to="/employees/new" className="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-blue-700">
            + Add Employee
          </Link>
        </div>

        {summary && (
          <div className="grid grid-cols-2 md:grid-cols-5 gap-4 mb-8">
            <SummaryCard title="Total Employees" value={summary.totalEmployees} />
            <SummaryCard title="Active Employees" value={summary.activeEmployees} />
            <SummaryCard title="Health Enrolled" value={summary.enrolledByBenefitType['Health'] ?? 0} />
            <SummaryCard title="Pending Enrollments" value={summary.pendingEnrollments} />
            <SummaryCard title="Enrollment Rate" value={formatPercent(summary.overallEnrollmentRate)} />
          </div>
        )}

        <div className="bg-white rounded-xl border border-gray-200 shadow-sm">
          <div className="p-4 border-b flex flex-wrap gap-3">
            <input
              type="text"
              placeholder="Search name or email..."
              value={search}
              onChange={e => handleSearch(e.target.value)}
              className="flex-1 min-w-48 px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <select
              value={department}
              onChange={e => handleDept(e.target.value)}
              className="px-3 py-2 border border-gray-300 rounded-md text-sm"
            >
              <option value="">All Departments</option>
              {DEPARTMENTS.map(d => <option key={d} value={d}>{d}</option>)}
            </select>
            <select
              value={benefitStatus}
              onChange={e => handleStatus(e.target.value)}
              className="px-3 py-2 border border-gray-300 rounded-md text-sm"
            >
              <option value="">All Statuses</option>
              {ENROLLMENT_STATUSES.map(s => (
                <option key={s} value={s}>{s === 'NotEnrolled' ? 'Not Enrolled' : s}</option>
              ))}
            </select>
          </div>

          {isLoading && <div className="p-8 text-center text-gray-500">Loading...</div>}
          {error && <div className="p-8 text-center text-red-500">Failed to load employees.</div>}

          {data && (
            <>
              <table className="w-full text-sm">
                <thead className="bg-gray-50 text-gray-600 uppercase text-xs tracking-wide">
                  <tr>
                    {['Name', 'Email', 'Department', 'Status', 'Actions'].map(h => (
                      <th key={h} className="px-4 py-3 text-left font-medium">{h}</th>
                    ))}
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-100">
                  {data.items.length === 0 && (
                    <tr>
                      <td colSpan={5} className="px-4 py-8 text-center text-gray-400">No employees found.</td>
                    </tr>
                  )}
                  {data.items.map(emp => (
                    <tr key={emp.id} className="hover:bg-gray-50 transition-colors">
                      <td className="px-4 py-3 font-medium text-gray-900">
                        <Link to={`/employees/${emp.id}`} className="hover:text-blue-600">
                          {emp.firstName} {emp.lastName}
                        </Link>
                      </td>
                      <td className="px-4 py-3 text-gray-600">{emp.email}</td>
                      <td className="px-4 py-3 text-gray-600">{emp.department}</td>
                      <td className="px-4 py-3"><Badge value={emp.employmentStatus} /></td>
                      <td className="px-4 py-3">
                        <div className="flex gap-2">
                          <Link to={`/employees/${emp.id}/edit`} className="text-blue-600 hover:underline text-xs">Edit</Link>
                          {deleteConfirm === emp.id ? (
                            <span className="flex gap-1">
                              <button onClick={() => confirmDelete(emp.id)} className="text-red-600 hover:underline text-xs">Confirm</button>
                              <button onClick={() => setDeleteConfirm(null)} className="text-gray-400 hover:underline text-xs">Cancel</button>
                            </span>
                          ) : (
                            <button onClick={() => setDeleteConfirm(emp.id)} className="text-red-400 hover:text-red-600 text-xs">Delete</button>
                          )}
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
              <div className="px-4 pb-4">
                <Pagination page={data.page} totalPages={data.totalPages} onPageChange={setPage} />
              </div>
            </>
          )}
        </div>
      </div>
    </div>
  );
}
