"use client"

import { useState } from 'react'
import { Badge } from "@/components/ui/badge"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { ArrowUpDown } from 'lucide-react'
import useAxiosPrivate from '@/hooks/useAxiosPrivate'
import useAuth from '@/hooks/useAuth'
import useUser from '@/hooks/useUser'
import { useRouter } from 'next/navigation';
import useSWR from 'swr'
import { ProfileType } from '@/api/common'
import getMyAlbums, { GetMyAlbumsResponse } from '@/api/users/getMyAlbums'
import Link from 'next/link'

export default function DashboardContent() {
  const { axiosPrivate, isReady } = useAxiosPrivate();
  const { isAuthenticated, auth } = useAuth();
  const { user, setUser } = useUser();
  const router = useRouter();
  const { data, isLoading } = useSWR(isReady && isAuthenticated && (user?.profileType === ProfileType.Artist || user?.profileType === ProfileType.Label) ? getMyAlbums.name : null, () => getMyAlbums(axiosPrivate, auth.userId!), {
    revalidateIfStale: true,
    revalidateOnFocus: false,
    revalidateOnReconnect: false
  });
  const [sortColumn, setSortColumn] = useState<keyof GetMyAlbumsResponse>('title')
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('asc')

  const sortedAlbums = data?.data ? [...data.data!].sort((a, b) => {
    if (a[sortColumn] < b[sortColumn]) return sortDirection === 'asc' ? -1 : 1
    if (a[sortColumn] > b[sortColumn]) return sortDirection === 'asc' ? 1 : -1
    return 0
  }) : [];

  const toggleSort = (column: keyof GetMyAlbumsResponse) => {
    if (column === sortColumn) {
      setSortDirection(sortDirection === 'asc' ? 'desc' : 'asc')
    } else {
      setSortColumn(column)
      setSortDirection('asc')
    }
  }

  return (
    <div className="container mx-auto py-10">
      <h1 className="text-2xl font-bold mb-6">Your music</h1>
      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead onClick={() => toggleSort('publicId')} className="cursor-pointer">
                Link <ArrowUpDown className="ml-2 h-4 w-4 inline" />
              </TableHead>
              <TableHead onClick={() => toggleSort('title')} className="cursor-pointer">
                Title <ArrowUpDown className="ml-2 h-4 w-4 inline" />
              </TableHead>
              <TableHead onClick={() => toggleSort('releaseDate')} className="cursor-pointer">
                Release Date <ArrowUpDown className="ml-2 h-4 w-4 inline" />
              </TableHead>
              <TableHead onClick={() => toggleSort('status')} className="cursor-pointer">
                Status <ArrowUpDown className="ml-2 h-4 w-4 inline" />
              </TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {sortedAlbums.map((album) => (
              <TableRow key={album.publicId}>
                <TableCell>
                  {album.status === "Released" ? <Link href={`/collections/${album.publicId.toLowerCase()}`} className="hover:underline">{album.publicId}</Link> : ""}
                </TableCell>
                <TableCell className='truncate max-w-[100px]'>{album.title}</TableCell>
                <TableCell>{album.releaseDate}</TableCell>
                <TableCell>
                  <Badge>
                    {album.status === "Submitted" ? "Processing" : album.status}
                  </Badge>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>
    </div>
  )
}