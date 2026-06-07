interface BadgeProps {
  value: string;
}

const colorMap: Record<string, string> = {
  Active: 'bg-green-100 text-green-800',
  Inactive: 'bg-gray-100 text-gray-600',
  Enrolled: 'bg-blue-100 text-blue-800',
  Pending: 'bg-yellow-100 text-yellow-800',
  Waived: 'bg-purple-100 text-purple-800',
  NotEnrolled: 'bg-red-100 text-red-800',
};

export default function Badge({ value }: BadgeProps) {
  const classes = colorMap[value] ?? 'bg-gray-100 text-gray-600';
  const label = value === 'NotEnrolled' ? 'Not Enrolled' : value;
  return (
    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${classes}`}>
      {label}
    </span>
  );
}
