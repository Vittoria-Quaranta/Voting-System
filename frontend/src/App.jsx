import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { VotingProvider } from './context/VotingContext';
import VotingHeader from './components/VotingHeader';
import Login from './pages/Login';
import Ballot from './pages/Ballot';
import Review from './pages/Review';
import Confirmation from './pages/Confirmation';
import VoteLookup from './pages/VoteLookup';
import ParticipationCheck from './pages/ParticipationCheck';
import Results from './pages/Results';
import DevTools from './pages/DevTools';

const isDev = import.meta.env.DEV;

function App() {
  return (
    <BrowserRouter>
      <VotingProvider>
        <VotingHeader />

        <main className="max-w-5xl mx-auto px-4 py-8">
          <Routes>
            <Route path="/" element={<Login />} />
            <Route path="/login" element={<Login />} />
            <Route path="/ballot" element={<Ballot />} />
            <Route path="/review" element={<Review />} />
            <Route path="/confirmation" element={<Confirmation />} />
            <Route path="/lookup" element={<VoteLookup />} />
            <Route path="/participation" element={<ParticipationCheck />} />
            <Route path="/results" element={<Results />} />
            {isDev && <Route path="/dev" element={<DevTools />} />}
          </Routes>
        </main>
      </VotingProvider>
    </BrowserRouter>
  );
}

export default App;
