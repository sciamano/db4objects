package com.db4o.internal.activation;

import com.db4o.internal.ClassMetadata;

public class UnknownActivationDepth implements ActivationDepth {
	
	public static final ActivationDepth INSTANCE = new UnknownActivationDepth();
	
	private UnknownActivationDepth() {
	}

	public ActivationDepth descend(ClassMetadata metadata, ActivationMode mode) {
		throw new IllegalStateException();
	}

	public boolean requiresActivation() {
		throw new IllegalStateException();
	}

}
