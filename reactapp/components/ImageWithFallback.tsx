import { imageLoader } from '@/libs/helpers'
import Image, { ImageProps } from 'next/image'
import { useState } from 'react'

interface ImageWithFallbackProps extends Omit<ImageProps, 'src'> {
  src: string
  fallbackSrc: string
  isProfile?: boolean
}

export default function ImageWithFallback({
  src,
  fallbackSrc,
  isProfile = false,
  alt,
  width,
  height,
  className,
  ...rest
}: ImageWithFallbackProps) {
  const [imgSrc, setImgSrc] = useState(src)

  return (
    <Image
      {...rest}
      src={imgSrc}
      alt={alt}
      width={width}
      height={height}
      className={className}
      loader={imageLoader(fallbackSrc)}
      onError={() => {
        setImgSrc(fallbackSrc)
      }}
    />
  )
}

