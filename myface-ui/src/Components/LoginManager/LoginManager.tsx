import React, { createContext, ReactNode, useState } from "react";
interface LoginContextType {
  isLoggedIn: boolean;
  isAdmin: boolean;
  logIn(): void;
  logOut(): void;
  username: string;
  setUsername(args0: string): void;
  password: string;
  setPassword(args0: string): void;
}
export const LoginContext = createContext<LoginContextType>({
  isLoggedIn: false,
  isAdmin: false,
  logIn: () => {},
  logOut: () => {},
  username: "",
  setUsername: () => {},
  password: "",
  setPassword: () => {},
});

interface LoginManagerProps {
  children: ReactNode;
}

export function LoginManager(props: LoginManagerProps): JSX.Element {
  const [loggedIn, setLoggedIn] = useState(false);
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  function logIn() {
    setLoggedIn(true);
  }

  function logOut() {
    setLoggedIn(false);
  }

  const context = {
    isLoggedIn: loggedIn,
    isAdmin: loggedIn,
    logIn: logIn,
    logOut: logOut,
    username: username,
    setUsername: setUsername,
    password: password,
    setPassword:  setPassword,
  };

  return (
    <LoginContext.Provider value={context}>
      {props.children}
    </LoginContext.Provider>
  );
}
