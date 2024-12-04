'use client'

import { use, useEffect, useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'
import { Button } from "@/components/ui/button"
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Instagram, Mail } from 'lucide-react'
import Link from "next/link"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import PurpleButton from '@/components/Button'
import { useToast } from '@/hooks/use-toast'
import useAuth from '@/hooks/useAuth'
import useUser from '@/hooks/useUser'
import submitContactForm from '@/api/users/submitContactForm'

const formSchema = z.object({
  name: z.string().min(2, {
    message: "Name must be at least 2 characters.",
  }).max(100),
  email: z.string().email({
    message: "Please enter a valid email address.",
  }).max(100),
  topic: z.string({
    required_error: "Please select a topic.",
  }),
  description: z.string().min(10, {
    message: "Description must be at least 10 characters.",
  }).max(1000),
})

export default function ContactPage() {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { isAuthenticated, auth } = useAuth();
  const user = useUser();
  const { toast } = useToast()

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      name: isAuthenticated && user?.user?.name ? user.user.name : "",
      email: isAuthenticated && auth.email ? auth.email : "",
      topic: "",
      description: "",
    },
  })

  useEffect(() => {
    if (isAuthenticated && auth.email) {
      form.setValue("email", auth.email);
    }
    if (isAuthenticated && user?.user?.name) {
      form.setValue("name", user.user.name);
    }
  }, [isAuthenticated, auth.email, user?.user?.name, form]);

  async function onSubmit(values: z.infer<typeof formSchema>) {
    setIsSubmitting(true);
    const response = await submitContactForm(values.email, values.name, values.topic, values.description);
    setIsSubmitting(false);
    if (response.ok) {
      toast({
        title: "Form submitted",
        description: "We've received your message and will get back to you soon.",
      })
      form.setValue("topic", "");
      form.setValue("description", "");
    }
  }

  return (
    <div className="px-4 sm:px-6 lg:px-8">
      <Card className="mb-12">
        <CardHeader>
          <CardTitle className="text-2xl">Quick Connect</CardTitle>
          <CardDescription>Reach out directly for specific inquiries</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <Link
            href="mailto:hello@noizera.com"
            className="flex items-center text-primary text-lg hover:underline"
          >
            <Mail className="mr-3 h-5 w-5" />
            General Inquiries: hello@noizera.com
          </Link>
          <Link
            href="mailto:legal@noizera.com"
            className="flex items-center text-primary text-lg hover:underline"
          >
            <Mail className="mr-3 h-5 w-5" />
            Legal Issues: legal@noizera.com
          </Link>
          <Link
            href="https://www.instagram.com/noizeraa/"
            target="_blank"
            rel="noopener noreferrer"
            className="flex items-center text-primary text-lg hover:underline"
          >
            <svg xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
              className="w-5 h-5 mr-3"><title>Instagram</title><path d="M7.0301.084c-1.2768.0602-2.1487.264-2.911.5634-.7888.3075-1.4575.72-2.1228 1.3877-.6652.6677-1.075 1.3368-1.3802 2.127-.2954.7638-.4956 1.6365-.552 2.914-.0564 1.2775-.0689 1.6882-.0626 4.947.0062 3.2586.0206 3.6671.0825 4.9473.061 1.2765.264 2.1482.5635 2.9107.308.7889.72 1.4573 1.388 2.1228.6679.6655 1.3365 1.0743 2.1285 1.38.7632.295 1.6361.4961 2.9134.552 1.2773.056 1.6884.069 4.9462.0627 3.2578-.0062 3.668-.0207 4.9478-.0814 1.28-.0607 2.147-.2652 2.9098-.5633.7889-.3086 1.4578-.72 2.1228-1.3881.665-.6682 1.0745-1.3378 1.3795-2.1284.2957-.7632.4966-1.636.552-2.9124.056-1.2809.0692-1.6898.063-4.948-.0063-3.2583-.021-3.6668-.0817-4.9465-.0607-1.2797-.264-2.1487-.5633-2.9117-.3084-.7889-.72-1.4568-1.3876-2.1228C21.2982 1.33 20.628.9208 19.8378.6165 19.074.321 18.2017.1197 16.9244.0645 15.6471.0093 15.236-.005 11.977.0014 8.718.0076 8.31.0215 7.0301.0839m.1402 21.6932c-1.17-.0509-1.8053-.2453-2.2287-.408-.5606-.216-.96-.4771-1.3819-.895-.422-.4178-.6811-.8186-.9-1.378-.1644-.4234-.3624-1.058-.4171-2.228-.0595-1.2645-.072-1.6442-.079-4.848-.007-3.2037.0053-3.583.0607-4.848.05-1.169.2456-1.805.408-2.2282.216-.5613.4762-.96.895-1.3816.4188-.4217.8184-.6814 1.3783-.9003.423-.1651 1.0575-.3614 2.227-.4171 1.2655-.06 1.6447-.072 4.848-.079 3.2033-.007 3.5835.005 4.8495.0608 1.169.0508 1.8053.2445 2.228.408.5608.216.96.4754 1.3816.895.4217.4194.6816.8176.9005 1.3787.1653.4217.3617 1.056.4169 2.2263.0602 1.2655.0739 1.645.0796 4.848.0058 3.203-.0055 3.5834-.061 4.848-.051 1.17-.245 1.8055-.408 2.2294-.216.5604-.4763.96-.8954 1.3814-.419.4215-.8181.6811-1.3783.9-.4224.1649-1.0577.3617-2.2262.4174-1.2656.0595-1.6448.072-4.8493.079-3.2045.007-3.5825-.006-4.848-.0608M16.953 5.5864A1.44 1.44 0 1 0 18.39 4.144a1.44 1.44 0 0 0-1.437 1.4424M5.8385 12.012c.0067 3.4032 2.7706 6.1557 6.173 6.1493 3.4026-.0065 6.157-2.7701 6.1506-6.1733-.0065-3.4032-2.771-6.1565-6.174-6.1498-3.403.0067-6.156 2.771-6.1496 6.1738M8 12.0077a4 4 0 1 1 4.008 3.9921A3.9996 3.9996 0 0 1 8 12.0077" /></svg>
            <span>DM us on Instagram | @noizeraa</span>
          </Link>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-2xl">Contact Form</CardTitle>
          <CardDescription>Fill out the form below and we'll get back to you as soon as possible.</CardDescription>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-8">
              <FormField
                control={form.control}
                name="name"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-base">Name</FormLabel>
                    <FormControl>
                      <Input placeholder="Your name" {...field} className="text-base p-6" />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="email"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-base">Email</FormLabel>
                    <FormControl>
                      <Input disabled={isAuthenticated} type="email" placeholder="Your email" {...field} className="text-base p-6" />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="topic"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-base">Topic</FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger className="text-base p-6">
                          <SelectValue placeholder="Select a topic" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="General inquiries">General inquiries</SelectItem>
                        <SelectItem value="Technical issues">Technical issues</SelectItem>
                        <SelectItem value="Billing, royalties and subscriptions">Billing, royalties and subscriptions</SelectItem>
                        <SelectItem value="Account management">Account management</SelectItem>
                        <SelectItem value="Product questions">Product questions</SelectItem>
                        <SelectItem value="Partnerships and Business Inquiries">Partnerships and Business Inquiries</SelectItem>
                        <SelectItem value="Other questions">Other questions</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="description"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-base">Description</FormLabel>
                    <FormControl>
                      <Textarea
                        placeholder="Please describe your problem or inquiry"
                        className="resize-none text-base p-6"
                        {...field}
                        rows={6}
                      />
                    </FormControl>
                    <FormDescription className="text-sm">
                      Please provide as much detail as possible.
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <PurpleButton type="submit" disabled={isSubmitting} className="w-full text-base py-6">
                {isSubmitting ? "Submitting..." : "Submit"}
              </PurpleButton>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  )
}

