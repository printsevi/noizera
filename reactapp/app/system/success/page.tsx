import SuccessContent from './components/SuccessContent';

interface Props {
  params: { session_id: string };
}

const Success = async ({ params }: Props) => {
  return <SuccessContent sessionId={params.session_id} />;
};

export default Success;
