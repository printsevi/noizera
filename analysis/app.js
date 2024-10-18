import express from 'express';
import multer from 'multer';
import fs from 'fs';
import { Essentia, EssentiaWASM } from 'essentia.js';
import decode from 'audio-decode';

const app = express();
const port = 3005;

const essentia = new Essentia(EssentiaWASM);

const KEYS = ['C', 'D', 'E', 'F', 'G', 'A', 'B'];

app.use(express.json());
app.use(express.urlencoded({ extended: true }));

const decodeAudio = async (filepath) => {
  const buffer = fs.readFileSync(filepath);
  const audio = await decode(buffer);
  console.log(audio);
  const audioVector = essentia.arrayToVector(audio.getChannelData(0));
  console.log(audioVector);
  return audioVector;
};

const storage = multer.diskStorage({
    destination: (req, file, cb) => {
      cb(null, 'uploads/');
    },
    filename: (req, file, cb) => {
      cb(null, Date.now() + '-' + file.originalname);
    },
});
  
const upload = multer({ dest: "uploads/", storage: storage });

app.post('/upload', upload.single('file'), async (req, res) => {
    if (!req.file) {
        return res.status(400).json({ error: 'No file uploaded' });
    }

    const data = await decodeAudio(req.file.path);

    const danceability = essentia.Danceability(data).danceability;
    const duration = essentia.Duration(data).duration;
    const energy = essentia.Energy(data).energy;

    const computedKey = essentia.KeyExtractor(data);
    const key = KEYS.indexOf(computedKey.key);
    const mode = computedKey.scale === 'major' ? 1 : 0;

    const loudness = essentia.DynamicComplexity(data).loudness;
    const tempo = essentia.PercivalBpmEstimator(data).bpm;

    res.status(200).json({
        danceability,
        duration,
        energy,
        key,
        mode,
        loudness,
        tempo,
    });
});

app.listen(port, () => {
  return console.log(`Express server listening at http://localhost:${port}`);
});