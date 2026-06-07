interface SummaryCardProps {
  title: string;
  value: string | number;
  subtitle?: string;
}

export default function SummaryCard({ title, value, subtitle }: SummaryCardProps) {
  return (
    <div className="bg-white rounded-lg border border-gray-200 p-5 shadow-sm">
      <p className="text-sm font-medium text-gray-500">{title}</p>
      <p className="mt-1 text-3xl font-semibold text-gray-900">{value}</p>
      {subtitle && <p className="mt-1 text-sm text-gray-400">{subtitle}</p>}
    </div>
  );
}
