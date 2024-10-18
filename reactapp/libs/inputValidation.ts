export function findInputError(errors: any, name: string) {
    const filtered = Object.keys(errors)
      .filter(key => key.includes(name))
      .reduce((cur, key) => {
        return Object.assign(cur, { error: errors[key] })
      }, {})
    return filtered as any;
}

export const isFormInvalid = (err: Object) => {
    if (Object.keys(err).length > 0) return true
    return false
  }

export const name_validation = {
  name: 'name',
  label: 'name',
  type: 'text',
  id: 'name',
  placeholder: 'Type your username',
  validation: {
    required: {
      value: true,
      message: 'required',
    },
    maxLength: {
      value: 30,
      message: '30 characters max',
    },
  },
}

export const checkbox_validation = {
  type: 'checkbox',
  validation: {
    required: {
      value: true,
      message: 'required',
    },
  },
}
  
  export const desc_validation = {
    name: 'description',
    label: 'description',
    multiline: true,
    id: 'description',
    placeholder: 'write description ...',
    validation: {
      required: {
        value: true,
        message: 'required',
      },
      maxLength: {
        value: 200,
        message: '200 characters max',
      },
    },
  }
  
  export const password_validation = {
    name: 'password',
    label: 'password',
    type: 'password',
    id: 'password',
    placeholder: 'Type your password',
    validation: {
      required: {
        value: true,
        message: 'Password is required',
      },
      pattern: {
        value:
        /^(?=.*?[0-9])(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^0-9A-Za-z]).{8,128}$/,
        message: 'Password is not strong enough',
      },
    },
  }
  
  export const num_validation = {
    name: 'num',
    label: 'number',
    type: 'number',
    id: 'num',
    placeholder: 'write a random number',
    validation: {
      required: {
        value: true,
        message: 'required',
      },
    },
  }
  
  export const email_validation = {
    name: 'email',
    label: 'Email',
    type: 'email',
    id: 'email',
    placeholder: 'Type Your Email',
    validation: {
      required: {
        value: true,
        message: 'email is required',
      },
      pattern: {
        value:
          /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
        message: 'Email is not valid',
      },
    },
  }

  export const PASSWORD_REGEX = RegExp(/^(?=.*?[0-9])(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^0-9A-Za-z]).{8,128}$/);