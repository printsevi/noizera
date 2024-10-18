import { Slider } from "./ui/slider";

interface ProgressCSSProps extends React.CSSProperties {
    '--progress-width': number;
    '--buffered-width': number;
  }
  
  interface AudioProgressBarProps {
    duration: number;
    currentProgress: number;
    buffered: number;
    onValueChange: (value: number) => void;
  }
  
  export default function AudioProgressBar(props: AudioProgressBarProps) {
    const { duration, currentProgress, buffered, onValueChange, ...rest } = props;
  
    const progressBarWidth = isNaN(currentProgress / duration)
      ? 0
      : currentProgress / duration;
    const bufferedWidth = isNaN(buffered / duration) ? 0 : buffered / duration;
  
    const progressStyles: ProgressCSSProps = {
      '--progress-width': progressBarWidth,
      '--buffered-width': bufferedWidth,
    };
  
    return (
      <div className="absolute h-1 -top-[2px] left-0 right-0 group">
        <Slider 
            defaultValue={[0]}
            min={0}
            max={duration}
            step={0.01}
            value={[currentProgress]}
            onValueChange={(e) => onValueChange(e[0])} 
          />
      </div>
    );
  }
  